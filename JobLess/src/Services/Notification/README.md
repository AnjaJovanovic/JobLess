# Notification mikroservis (JobLess.Notification)

Mikroservis zadužen za obaveštavanje korisnika (kandidata i kompanija) o događajima na platformi — kroz in-app obaveštenja (REST API) i email. Servis ne inicira sam nijedan poslovni događaj — isključivo **reaguje** na događaje koje objavljuju drugi mikroservisi preko RabbitMQ-a (MassTransit), i izlaže REST API kojim frontend čita/ažurira obaveštenja konkretnog korisnika.

## Odgovornosti

- Pretplata na tri domenska događaja sa RabbitMQ-a i kreiranje odgovarajućih `Notification` zapisa u bazi
- Slanje email poruke dobrodošlice pri registraciji korisnika
- Izlaganje REST API-ja (zaštićenog JWT-om) za čitanje sopstvenih obaveštenja i označavanje kao pročitano

## Arhitektura

```
JobLess.Notification.API             → NotificationsController, Program.cs (kompozicija, MassTransit setup)
JobLess.Notification.Application     → MediatR komande, MassTransit konzumeri, interfejsi
JobLess.Notification.Domain          → Notification entitet, NotificationType enum
JobLess.Notification.Infrastructure  → NotificationDbContext (EF Core), EmailService (SMTP)
```

Dva nezavisna ulaza u servis:

1. **RabbitMQ konzumeri** (`*Consumer` klase) — primaju domenske događaje od Auth i JobApplication servisa i upisuju obaveštenja direktno u bazu (mimo MediatR-a).
2. **REST API** (`NotificationsController`) — čita/menja obaveštenja ulogovanog korisnika, kroz MediatR komande.

Dijagram klasa: [JobLess/docs/diagrams/notification_dijagram_klasa.drawio](../../../docs/diagrams/notification_dijagram_klasa.drawio)

## Događaji koje servis konzumira

Svi događaji stižu preko fanout exchange-eva na RabbitMQ-u; svaki ima svoj red (queue) koji sluša odgovarajući konzument (`Program.cs`).

| Poruka | Objavljuje | Queue | Konzument | Efekat |
|---|---|---|---|---|
| `UserRegisteredMessage(UserId, Email, Role)` | Auth servis | `notification-user-registered-queue` | `UserRegisteredConsumer` | Kreira obaveštenje tipa `Welcome` + šalje email dobrodošlice |
| `JobAppliedMessage(ApplicationId, AdvertisementId, ClientId, ClientEmail, ClientFirstName, ClientLastName, CompanyId, CompanyEmail)` | JobApplication servis | `notification-job-applied-queue` | `JobAppliedConsumer` | Kreira obaveštenje tipa `NewApplication` za kompaniju |
| `ApplicationStatusChangedMessage(ApplicationId, AdvertisementId, CompanyId, CandidateEmail, CandidateFirstName, CandidateLastName, NewStatus)` | JobApplication servis | `notification-application-status-queue` | `ApplicationStatusChangedConsumer` | Kreira obaveštenje tipa `ApplicationAccepted`/`ApplicationRejected` za kandidata (na osnovu `NewStatus == "Accepted"`) |

Kontrakti poruka su definisani u zajedničkom projektu `JobLess.Contracts` (`src/Shared/JobLess.Contracts/Events`), koji dele i servis-pošiljalac i Notification servis kao primalac.

Greška pri slanju email-a (u `UserRegisteredConsumer`) se hvata i loguje, ali **ne prekida** obradu poruke — in-app obaveštenje se svakako upiše u bazu čak i ako SMTP zahtev ne uspe.

## REST API

Base ruta: `/api/notifications` (kroz Gateway: `http://localhost:5000/api/notifications`). Svi endpoint-i zahtevaju važeći JWT (`[Authorize]`); korisnik se identifikuje preko `email` claim-a iz tokena, a ne preko URL parametra — korisnik uvek vidi samo svoja obaveštenja.

### `GET /api/notifications/me`

Vraća sva obaveštenja ulogovanog korisnika, sortirana po datumu kreiranja (najnovije prvo).

**Response `200 OK`**
```json
[
  {
    "id": "5f3e...",
    "recipientUserId": "kompanija@example.com",
    "title": "Nova prijava na oglas",
    "message": "Kandidat Marko Petrović (marko@example.com) se prijavio na vas oglas.",
    "isRead": false,
    "createdAt": "2026-07-13T09:12:00Z",
    "type": 1,
    "applicationId": 42,
    "advertisementId": 7,
    "candidateId": 3,
    "companyId": 1
  }
]
```

### `PUT /api/notifications/{id}/read`

Označava jedno obaveštenje kao pročitano (samo ako pripada ulogovanom korisniku).

**Response** `204 No Content` (uspeh) / `401 Unauthorized` / `404 Not Found` (obaveštenje ne postoji ili ne pripada korisniku).

## Model podataka

`Notification` (tabela u bazi `JobLessNotificationDb`):

| Kolona | Tip | Napomena |
|---|---|---|
| `Id` | `Guid` | primarni ključ |
| `RecipientUserId` | `string` | email primaoca (koristi se kao identifikator, ne interni ID) |
| `Title`, `Message` | `string` (max 200 / 1000) | sadržaj obaveštenja |
| `IsRead` | `bool` | status čitanja |
| `CreatedAt` | `DateTime` | vreme kreiranja |
| `Type` | `NotificationType` | `Welcome`, `NewApplication`, `ApplicationAccepted`, `ApplicationRejected` |
| `ApplicationId`, `AdvertisementId`, `CandidateId`, `CompanyId` | `int?` | opciona metapolja da bi frontend mogao da prikaže detalje vezanog oglasa/prijave |

## Slanje email poruka

`EmailService` (Infrastructure sloj) koristi **MailKit/MimeKit** i šalje HTML + tekstualnu email poruku dobrodošlice preko SMTP-a (podrazumevano Gmail SMTP, `StartTls`, port 587). Sadržaj poruke se prilagođava ulozi korisnika (kandidat vs. kompanija).

## Konfiguracija

| Promenljiva | Opis |
|---|---|
| `ConnectionStrings:DefaultConnection` | konekcija ka `JobLessNotificationDb` |
| `Jwt:Key` / `Issuer` / `Audience` | moraju biti identični sa Auth servisom (validacija tokena) |
| `RabbitMq:Host` / `Username` / `Password` | konekcija ka brokeru |
| `Smtp:Host` / `Port` / `UserName` / `Password` / `FromEmail` / `FromName` | SMTP nalog za slanje email-a |

> **Bezbednosna napomena:** trenutne SMTP i JWT vrednosti u `appsettings.json`/`docker-compose.yml` su demo kredencijali za potrebe seminarskog rada. Pre bilo kakvog javnog objavljivanja repozitorijuma preporučuje se rotacija Gmail app-password-a i premeštanje svih tajni u environment/secret store van verzionisanog koda.

CORS je ograničen na `http://localhost:5173`.


## Pokretanje

Videti opšte uputstvo u [`docs/POKRETANJE.md`](../../../../docs/POKRETANJE.md). Za lokalno pokretanje van Dockera, pokrenuti SQL Server i RabbitMQ:

```bash
cd JobLess
docker compose up -d sql-server rabbitmq
docker compose ps   # sačekati da oba budu "healthy"

cd src/Services/Notification/JobLess.Notification.API
dotnet run
```
## Ručno testiranje (Swagger i terminal)

Endpointi su zaštićeni (`[Authorize]`) — potreban je važeći JWT `accessToken`, dobijen prijavom preko Auth servisa (videti [`src/Security/README.md`](../../Security/README.md#ručno-testiranje-swagger-i-terminal)).

### Preko Swaggera

1. Otvoriti Swagger UI servisa (`http://localhost:5199/swagger`).
2. Kliknuti na katanac (**Authorize**) gore desno i uneti `Bearer <accessToken>` (token dobijen sa `/api/Auth/login` ili `/api/Auth/register`).
3. Razviti `GET /api/notifications/me` → **"Try it out"** → **"Execute"** — vraća listu obaveštenja ulogovanog korisnika.
4. Za `PUT /api/notifications/{id}/read` uneti `id` obaveštenja (iz prethodnog odgovora) i izvršiti — vraća `204 No Content`.

### Preko terminala (curl)

```bash
TOKEN="<accessToken dobijen od Auth servisa>"

# Lista obaveštenja ulogovanog korisnika
curl -s http://localhost:5199/api/notifications/me \
  -H "Authorization: Bearer $TOKEN"

# Označavanje obaveštenja kao pročitanog
curl -s -X PUT http://localhost:5199/api/notifications/<ID_OBAVESTENJA>/read \
  -H "Authorization: Bearer $TOKEN"
```

Da bi se u bazi pojavilo bar jedno obaveštenje za test, dovoljno je registrovati korisnika preko Auth servisa (`POST /api/Auth/register`) — `UserRegisteredConsumer` će automatski kreirati obaveštenje tipa `Welcome` za taj email.

## Automatizovani testovi

Test projekat: `src/Tests/JobLess.Tests.Notification` (xUnit), pokriva:

- `UserRegisteredConsumerTests`, `JobAppliedConsumerTests`, `ApplicationStatusChangedConsumerTests` — obrada RabbitMQ poruka
- `GetUserNotificationsQueryHandlerTests`, `MarkNotificationAsReadCommandHandlerTests` — Application sloj (MediatR handleri)

```bash
dotnet test src/Tests/JobLess.Tests.Notification
```
