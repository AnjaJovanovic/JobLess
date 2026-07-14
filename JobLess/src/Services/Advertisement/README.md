# Advertisement mikroservis (JobLess.Advertisement)
 
Mikroservis zadužen za oglase za posao  na JobLess platformi — kreiranje, izmenu, aktivaciju, brisanje i pregled oglasa. Kompanije objavljuju i upravljaju svojim oglasima, kandidati pregledaju i pretražuju aktivne oglase.
 
## Odgovornosti
 
- Kreiranje oglasa (samo uloga **Company**)
- Izmena postojećeg oglasa (samo vlasnik-kompanija)
- Aktivacija oglasa (samo vlasnik-kompanija)
- Brisanje oglasa (samo vlasnik-kompanija)
- Prikaz svih (aktivnih) oglasa kandidatima, sa paginacijom
- Prikaz pojedinačnog oglasa (kandidatu ili kompaniji)
- Pretraga oglasa (kandidat)
- Prikaz svih oglasa određene kompanije, samo njoj (kompanija vidi samo svoje oglase)
Svi endpoint-i su zaštićeni (`[Authorize]`), autorizacija je bazirana na roli iz JWT tokena (`Candidate` / `Company`), a identitet kompanije se uzima iz `ClaimTypes.Email` u tokenu (ne šalje se u telu zahteva).
 
## Arhitektura
 
Clean Architecture, 4 projekta:
 
```
JobLess.Advertisement.API             → kontroleri (AdvertisementsController), Program.cs
JobLess.Advertisement.Application     → MediatR komande/upiti (CQRS), validatori (FluentValidation), modeli/DTO-ovi
JobLess.Advertisement.Domain          → JobAdvertisement entitet, enumi
JobLess.Advertisement.Infrastructure  → EF Core (ApplicationDbContext)
```
 
Tok zahteva: `AdvertisementsController` → `IMediator` → `*CommandHandler` / `*QueryHandler` (Application) → `IApplicationDbContext` (EF Core, SQL Server).
 
Servis **ne** šalje niti sluša događaje preko RabbitMQ/MassTransit.
 
## Model podataka
 
Entitet `JobAdvertisement : BaseEntity`:
 
| Polje | Tip | Opis |
|---|---|---|
| `Id` | `int` | (iz `BaseEntity`) |
| `CompanyId` | `int` | ID kompanije koja je vlasnik oglasa |
| `CompanyEmail` | `string` | Email kompanije (iz JWT tokena, koristi se za autorizaciju vlasništva) |
| `Title` | `string` | Naziv oglasa |
| `Description` | `string?` | Opis (do 5000 karaktera) |
| `Position` | `string` | Naziv pozicije |
| `PostedAt` | `DateTime` | Datum objave (postavlja se automatski na `UtcNow` pri kreiranju) |
| `ExpiresAt` | `DateTime?` | Datum isteka (mora biti u budućnosti) |
| `IsActive` | `bool` | Da li je oglas aktivan (`true` po defaultu pri kreiranju) |
| `EmploymentType` | `EmploymentType` (enum) | Tip zaposlenja |
| `WorkSchedule` | `WorkSchedule` (enum) | Raspored rada |
| `SeniorityLevel` | `SeniorityLevel` (enum) | Nivo iskustva |
| `MinExperience` / `MaxExperience` | `int?` | Opseg godina iskustva (min ≤ max) |
| `City` | `string` | Grad (obavezno) |
| `Country` | `string?` | Država (do 100 karaktera) |
| `WorkType` | `WorkType` (enum) | Tip rada |
| `SalaryFrom` / `SalaryTo` | `decimal?` | Opseg plate (from ≤ to, oba > 0) |
| `Currency` | `string?` | Valuta (obavezna ako je plata uneta, do 10 karaktera) |
| `IsSalaryVisible` | `bool` | Da li je plata vidljiva (default `true`; ako je `true`, mora postojati bar `SalaryFrom` ili `SalaryTo`) |

 
### Enumi
 
```csharp
public enum EmploymentType { Permanent = 0, Temporary = 1, Internship = 2 }
public enum WorkSchedule   { Full = 0, PartTime = 1 }
public enum SeniorityLevel { Beginner = 0, Junior = 1, Medior = 2, Senior = 3 }
public enum WorkType       { OnSite = 0, Remote = 1, Hybrid = 2 }
```
 
## REST API
 
Base ruta: `/api/Advertisements` (kroz Gateway: `http://localhost:5000/api/Advertisements`, direktno kroz Docker: `http://localhost:5104/api/Advertisements`).
 
Svi endpoint-i zahtevaju `Authorization: Bearer <accessToken>`.
 
### `POST /api/Advertisements` — Kreiranje oglasa
**Rola:** `Company`
 
**Request**
```json
{
  "companyId": 1,
  "title": "Backend Developer",
  "position": "Backend Developer",
  "description": "Opis pozicije...",
  "expiresAt": "2026-09-01T00:00:00Z",
  "employmentType": 0,
  "workSchedule": 0,
  "seniorityLevel": 1,
  "minExperience": 1,
  "maxExperience": 3,
  "city": "Beograd",
  "country": "Srbija",
  "workType": 2,
  "salaryFrom": 1200,
  "salaryTo": 1800,
  "currency": "EUR",
  "isSalaryVisible": true
}
```
> `companyEmail` se **ne** šalje — postavlja se automatski iz JWT tokena prijavljene kompanije.
 
**Response `201 Created`** — telo: `int` (ID novog oglasa)
 
**Response `400 Bad Request`** — pri neispravnim podacima (validacija preko `CreateAdvertisementCommandValidator`, npr. nedostaje  naslov oglasa)
 
### `DELETE /api/Advertisements` — Brisanje oglasa
**Rola:** `Company`
 
**Request**
```json
{ "id": 12 }
```
 
**Response `200 OK`** — `true` / `false` (uspeh brisanja; `false` i ako kompanija nije vlasnik oglasa)
 
### `GET /api/Advertisements/All` — Svi oglasi (za kandidate)
**Rola:** `Candidate`
 
**Query parametri:** `pageNumber` (default `1`), `pageSize` (default `10`)
 
**Response `200 OK`** — `GetAllAdvertisementResult` :
```json
{
  "advertisements": [AdvertisementModel],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 4,
  "totalCount": 37
}
```
 
### `GET /api/Advertisements/One` — Pojedinačni oglas
**Rola:** `Candidate, Company`
 
**Query parametri:** `id`
 
**Response `200 OK`** — `GetOneAdvertisementResult` 
 
### `GET /api/Advertisements/Search` — Pretraga oglasa
**Rola:** `Candidate`
 
**Query parametri:** `SearchAdvertisementQuery` — svi filteri opcioni
 
| Parametar | Tip | Opis |
|---|---|---|
| `companyId` | `int?` | Filter po kompaniji |
| `title` | `string?` | Filter po nazivu oglasa |
| `position` | `string?` | Filter po poziciji |
| `expiresFrom` / `expiresTo` | `DateTime?` | Opseg datuma isteka |
| `employmentType` | `EmploymentType?` | Tip zaposlenja |
| `workSchedule` | `WorkSchedule?` | Raspored rada |
| `seniorityLevel` | `SeniorityLevel?` | Nivo iskustva |
| `minExperience` / `maxExperience` | `int?` | Opseg godina iskustva |
| `city` | `string?` | Grad |
| `country` | `string?` | Država |
| `workType` | `WorkType?` | Tip rada |
| `salaryFrom` / `salaryTo` | `decimal?` | Opseg plate |
| `currency` | `string?` | Valuta |
| `isSalaryVisible` | `bool?` | Da li je plata vidljiva |
| `pageNumber` | `int` | Default `1` |
| `pageSize` | `int` | Default `10` |
 
**Response `200 OK`** — `SearchAdvertisementResult` 
 
### `GET /api/Advertisements/GetAdvertisementsForCompany` — Oglasi prijavljene kompanije
**Rola:** `Company`
 
**Query parametri:**
 
| Parametar | Tip | Opis |
|---|---|---|
| `pageNumber` | `int` | Default `1` |
| `pageSize` | `int` | Default `10` |
| `companyId` | `int` | **Obavezno** |
 
> `companyEmail` se ne šalje — postavlja se automatski iz JWT tokena.
 
**Response `200 OK`**
```json
{
  "advertisements": [AdvertisementModel],
  "pageNumber": 1,
  "pageSize": 10,
  "totalPages": 2,
  "totalCount": 15
}
```
 
### `PUT /api/Advertisements/Activate` — Aktivacija oglasa
**Rola:** `Company`
 
**Query parametri:** `id`
 
**Response `200 OK`** — `true`
**Response `400 Bad Request`** — `false` (npr. oglas ne postoji ili kompanija nije vlasnik)
 
### `PUT /api/Advertisements/Update` — Izmena oglasa
**Rola:** `Company`
 
Sva polja osim `id` su opciona (parcijalni update — šalje se samo ono što se menja).
 
**Request**
```json
{
  "id": 12,
  "title": "Senior Backend Developer",
  "description": "Ažuriran opis pozicije...",
  "position": "Backend Developer",
  "expiresAt": "2026-10-01T00:00:00Z",
  "employmentType": 0,
  "workSchedule": 0,
  "seniorityLevel": 3,
  "minExperience": 3,
  "maxExperience": 6,
  "city": "Novi Sad",
  "country": "Srbija",
  "workType": 1,
  "salaryFrom": 1800,
  "salaryTo": 2400,
  "currency": "EUR",
  "isSalaryVisible": true
}
```
> `companyEmail` se ne šalje — postavlja se automatski iz JWT tokena i koristi za proveru vlasništva nad oglasom.
 
**Response `200 OK`**
 
## Autorizacija po ruti — pregled
 
| Endpoint | Rola |
|---|---|
| `POST /api/Advertisements` | Company |
| `DELETE /api/Advertisements` | Company |
| `GET /api/Advertisements/All` | Candidate |
| `GET /api/Advertisements/One` | Candidate, Company |
| `GET /api/Advertisements/Search` | Candidate |
| `GET /api/Advertisements/GetAdvertisementsForCompany` | Company |
| `PUT /api/Advertisements/Activate` | Company |
| `PUT /api/Advertisements/Update` | Company |
 

 
## Konfiguracija (Docker Compose)
 
```yaml
advertisement-service:
  build:
    context: .
    dockerfile: src/Services/Advertisement/JobLess.Advertisement.API/Dockerfile
  container_name: jobless-advertisement
  ports:
    - "5104:8080"
  environment:
    ASPNETCORE_ENVIRONMENT: "Development"
    ASPNETCORE_URLS: "http://+:8080"
    ConnectionStrings__DefaultConnection: >-
      Server=sql-server,1433;
      Database=JobLessDatabase;
      User Id=sa;
      Password=JobLess_Pass123!;
      TrustServerCertificate=True;
      MultipleActiveResultSets=True;
  depends_on:
    sql-server:
      condition: service_healthy
  networks:
    - jobless-network
  restart: on-failure
```
 
| Promenljiva | Opis |
|---|---|
| `ConnectionStrings__DefaultConnection` | Konekcija ka SQL Server bazi `JobLessDatabase` |
 
> Napomena: JWT konfiguracija (`Jwt:Key`/`Issuer`/`Audience`) nije eksplicitno navedena u compose bloku za ovaj servis (za razliku od Company/Notification/JobApplication servisa) — servis validira token isti potpisni ključ kao Auth servis, proveriti `appsettings.json` ili shared konfiguraciju ako endpoint-i vraćaju `401`.
 
## Pokretanje
 
Servis radi isključivo preko Dockera, kao deo cele JobLess Docker Compose infrastrukture (SQL Server, ostali servisi, API Gateway).
 
```bash
cd JobLess
docker compose up -d sql-server
```
 
Sačekati da `sql-server` bude `healthy`, zatim:
 
```bash
docker compose up -d advertisement-service
```
 
Za rad kroz Gateway (`http://localhost:5000/api/Advertisements`) potrebno je da i `api-gateway`, kao i `auth-service` (za dobijanje tokena), takođe budu pokrenuti:
 
```bash
docker compose up -d auth-service api-gateway
```
 
## Ručno testiranje
 
Svi pozivi zahtevaju validan JWT token dobijen sa Auth servisa (`POST /api/Auth/login`), sa odgovarajućom rolom (`Company` ili `Candidate`).
 
**Kreiranje oglasa (kompanija):**
```bash
curl -X POST http://localhost:5104/api/Advertisements \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <accessToken kompanije>" \
  -d '{
    "companyId": 1,
    "title": "Backend Developer",
    "position": "Backend Developer",
    "city": "Beograd",
    "employmentType": 0,
    "workSchedule": 0,
    "seniorityLevel": 1,
    "workType": 2
  }'
```
 
**Prikaz svih aktivnih oglasa (kandidat):**
```bash
curl "http://localhost:5104/api/Advertisements/All?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer <accessToken kandidata>"
```
 
## Automatizovani testovi
 
Test projekat: `src/Tests/JobLess.Tests.Advertisement` (unit testovi, po nekoliko test case-ova za svaku komandu — Create/Update/Delete/Activate handlere).
 
```bash
dotnet test src/Tests/JobLess.Tests.Advertisement
```
