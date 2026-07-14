# Company mikroservis — dokumentacija

Deo je **JobLess** platforme (mikroservisna arhitektura za oglašavanje poslova). Mikroservis `Company`
upravlja podacima o kompanijama koje objavljuju oglase za posao.

---

## 1. Pregled

| | |
|---|---|
| **Naziv projekta** | `JobLess.Company` |
| **Tip** | ASP.NET Core Web API (REST) + gRPC servis |
| **Arhitektura** | Clean Architecture (Domain / Application / Infrastructure / API) sa CQRS + MediatR |
| **Platforma** | .NET 8 |
| **Baza podataka** | Microsoft SQL Server (`JobLessCompanyDatabase`), Entity Framework Core 8 (Code-First, migracije) |
| **Autentifikacija** | JWT Bearer (od strane `Auth` servisa), autorizacija po rolama (`Company`) |
| **Komunikacija sa drugim servisima** | REST (preko API Gateway-a) i gRPC (interno, sinhrono) |
| **Kontejnerizacija** | Docker (poseban servis `company-service` u `docker-compose.yml`) |

Servis je deo veće mikroservisne arhitekture (`ApiGateway`, `Auth`, `Client`, `Advertisement`,
`Notification`, `JobApplication`, `Company`), gde `ApiGateway` (Ocelot) rutira `/api/Companies/*`
zahteve ka ovom servisu.

---

## 2. Arhitektura i slojevi

Projekat je organizovan po principu **Clean Architecture**, podeljen u 4 projekta:

```
JobLess.Company.Domain          → entiteti, enumi, poslovna pravila bez zavisnosti
JobLess.Company.Application     → CQRS komande/upiti, handleri, validatori, interfejsi
JobLess.Company.Infrastructure  → EF Core DbContext, migracije, pristup bazi
JobLess.Company.API             → REST kontroleri, gRPC servis, konfiguracija (Program.cs)
```

Zavisnosti idu isključivo ka unutra: `API → Infrastructure/Application → Domain`. `Domain` ne zavisi
ni od jednog drugog sloja.

### 2.1 Domain

Sadrži entitete i enume koji opisuju kompaniju kao poslovni koncept:

- **`Company`** — glavni entitet (nasleđuje `BaseEntity` iz `JobLess.Shared.Domain`). Sadrži:
  osnovne podatke (naziv, opis, industrija, veb sajt), pravne podatke (PIB — `TaxIdentificationNumber`,
  matični broj — `RegistrationNumber`), podatke o kontakt osobi, kontakt podatke kompanije
  (email, telefon, adresa, lokacija), veličinu kompanije (`CompanySize`) i status
  (`IsActive`, `CreatedAt`, `UpdatedAt`).
- **`Industry`** (enum) — delatnost kompanije (IT, finansije, maloprodaja, proizvodnja, zdravstvo,
  građevinarstvo, mediji, ostalo), sa lokalizovanim prikazom preko `DisplayAttribute`.
- **`CompanySize`** (enum) — opseg broja zaposlenih (1-10, 11-50, 51-200, 201-500, 500+).
- **`EnumExtensions`** — helper metoda `GetDisplayName()` koja čita `DisplayAttribute` sa enuma.

### 2.2 Application

Implementira CQRS pattern uz pomoć **MediatR** biblioteke — svaka operacija je komanda (upis) ili
upit (čitanje), sa odgovarajućim handlerom, i po potrebi validatorom (**FluentValidation**).

**Komande (write operacije):**

| Komanda | Opis |
|---|---|
| `CreateCompanyCommand` | Kreira novu kompaniju. Proverava duplikate po email-u, PIB-u ili matičnom broju. |
| `UpdateCompanyCommand` | Parcijalno ažurira postojeću kompaniju (menja samo polja koja su prosleđena i validna). |
| `DeleteCompanyCommand` | Soft delete — postavlja `IsActive = false`, ne briše fizički zapis. |

**Upiti (read operacije):**

| Upit | Opis |
|---|---|
| `GetAllCompaniesQuery` | Vraća listu svih aktivnih kompanija. |
| `GetOneCompanyQuery` | Vraća jednu kompaniju po `Id`. |
| `GetByNameCompanyQuery` | Pretraga po nazivu kompanije. |
| `SearchCompanyQuery` | Kombinovana pretraga po nazivu, industriji i lokaciji. |

**Ostalo:**

- **`IApplicationDbContext`** — apstrakcija nad EF Core `DbContext`-om, tako da `Application` sloj
  ne zavisi direktno od `Infrastructure`/EF Core implementacije.
- **`CompanyModel`** — DTO/projekcija entiteta `Company`, korišćena kao rezultat upita
  (`CompanyModel.Projection` je EF `Expression` koja se prevodi u SQL `SELECT`).
- **`ValidationBehavior<TRequest,TResponse>`** — MediatR *pipeline behavior* koji automatski pokreće
  FluentValidation validatore pre svakog handlera i baca `ValidationException` ako podaci nisu ispravni.
- **`IndustryHelper`** — mapira string vrednost industrije (na srpskom) u `Industry` enum i obrnuto,
  jer se industrija u komandama prima kao string, a čuva kao enum.

### 2.3 Infrastructure

- **`ApplicationDbContext`** — EF Core `DbContext`, implementira `IApplicationDbContext`. Sadrži
  `DbSet<Company>` i `DbSet<CompanyAdmin>`.
- **Migracije** — istorija šeme baze (hronološki): `Migracija1`, `NovaMigracija`,
  `InicijalnaMigracija`, `Migracija3`, `MigracijaNova`, `novaMigracija2`,
  `RemoveCompanyPasswordHash` (poslednja migracija uklanja kolonu `PasswordHash` iz tabele
  `Companies` — autentifikacija kompanije se više ne radi lozinkom unutar ovog servisa, već preko
  JWT tokena izdatog od strane `Auth` servisa).

### 2.4 API

- **`CompaniesController`** — REST kontroler (`/api/Companies`), štićen JWT autentifikacijom
  (`[Authorize]`), sa dodatnim ograničenjem na rolu `Company` za write operacije.
- **`CompanyProfileGrpcService`** — gRPC servis koji drugi mikroservisi (npr. `JobApplication`)
  koriste da sinhrono dobave osnovne podatke o kompaniji (`Id`, `Email`) po njenom ID-u, bez prolaska
  kroz REST/Gateway.
- **`Program.cs`** — konfiguracija: Kestrel (odvojeni HTTP i gRPC/HTTP2 port), CORS (za React
  frontend), JWT autentifikacija, EF Core + SQL Server, MediatR, FluentValidation,
  globalni exception handler (validacione greške → HTTP 400 sa listom poruka), automatska
  primena migracija (`dbContext.Database.Migrate()`) pri pokretanju.

---

## 3. REST API

Sve rute su relativne na `/api/Companies`. Kroz API Gateway se izlažu na istoj putanji
(`/api/Companies/{everything}`), sa rate-limitom od 20 zahteva/sekundi.

| Metoda | Ruta | Autorizacija | Opis |
|---|---|---|---|
| `POST` | `/api/Companies` | JWT, rola `Company` | Kreira novu kompaniju. Email vlasnika se uzima iz JWT tokena (claim), ne iz tela zahteva. |
| `PUT` | `/api/Companies/Update` | JWT, rola `Company` | Ažurira podatke kompanije ulogovanog korisnika. |
| `DELETE` | `/api/Companies` | JWT, rola `Company` | Soft-delete kompanije ulogovanog korisnika. |
| `GET` | `/api/Companies/All?pageNumber&pageSize` | Anonimno | Lista svih aktivnih kompanija. |
| `GET` | `/api/Companies/One?id=` | Anonimno | Detalji jedne kompanije. |
| `GET` | `/api/Companies/ByName?name=` | JWT (nasleđeno sa kontrolera) | Pretraga po nazivu. |
| `GET` | `/api/Companies/Search?Name&Industry&Location&PageNumber&PageSize` | JWT (nasleđeno) | Kombinovana pretraga sa filterima. |

> Napomena: rute `GetAll` i `GetOne` su eksplicitno označene sa `[AllowAnonymous]`, dok `ByName` i
> `Search` nemaju eksplicitnu anotaciju te nasleđuju `[Authorize]` sa nivoa kontrolera (zahtevaju bilo
> koji validan JWT, bez specifične role).

### Validacija (FluentValidation)

- **Kreiranje**: naziv (obavezno, ≤200 karaktera), industrija (mora biti jedna od unapred definisanih
  vrednosti preko `IndustryHelper`), veličina kompanije (mora biti validna enum vrednost), lokacija
  (obavezno, ≤200), opis (≤2000), veb sajt (validan URL, ≤300), PIB (tačno 9 cifara), matični broj
  (tačno 8 cifara), podaci o kontakt osobi (ime/prezime ≤30, pozicija ≤50), email (obavezan, validan
  format), telefoni (regex format `+381 60 123 4567`).
- **Ažuriranje**: ista pravila primenjena samo na polja koja su poslata (svi validatori su opcioni/`When`).

---

## 4. gRPC API

Definisano u `company_profile.proto` (deljeni `JobLess.Grpc.Contracts` projekat):

```protobuf
service CompanyProfileGrpc {
  rpc GetById (GetByIdRequest) returns (CompanyProfileReply);
}
message GetByIdRequest { int32 company_id = 1; }
message CompanyProfileReply {
  bool found = 1;
  int32 company_id = 2;
  string email = 3;
}
```

Koristi se za interne, sinhrone pozive između mikroservisa (npr. `JobApplication` servis proverava
da li kompanija postoji i uzima njen email pre slanja obaveštenja), bez prolaska kroz API Gateway.

---

## 5. Model podataka

### Tabela `Companies`

| Kolona | Tip | Napomena |
|---|---|---|
| Id | int (PK) | |
| Name | nvarchar | obavezno |
| Description | nvarchar | opciono |
| Industry | int (enum) | |
| Website | nvarchar | opciono |
| TaxIdentificationNumber | nvarchar | PIB, 9 cifara |
| RegistrationNumber | nvarchar | matični broj, 8 cifara |
| OwnerName | nvarchar | opciono |
| ContactPersonFirstName / LastName / Position / PhoneNumber | nvarchar | obavezno |
| Email | nvarchar | obavezno, jedinstveno u kombinaciji sa aktivnošću |
| PhoneNumber | nvarchar | opciono |
| Location | nvarchar | obavezno |
| Address | nvarchar | opciono |
| CompanySize | int (enum) | |
| IsActive | bit | koristi se za soft delete |
| CreatedAt / UpdatedAt | datetime2 | |

### Tabela `CompanyAdmins`

| Kolona | Tip |
|---|---|
| Id | int (PK) |
| CompanyId | int |
| UserId | int |
| Role | nvarchar |
| CreatedAt | datetime2 |

---

## 6. Konfiguracija i pokretanje

### `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=JobLessCompanyDatabase;..."
  },
  "Jwt": { "Key": "...", "Issuer": "JobLess.Auth", "Audience": "JobLess.Client" },
  "Kestrel": { "HttpPort": 5287, "GrpcPort": 5288 }
}
```

### Docker / docker-compose

Servis `company-service` je definisan u korenskom `docker-compose.yml`:

- HTTP port: `5287 → 8080`
- gRPC port: `5288 → 8081`
- Povezan na `sql-server` servis 
- Environment varijable override-uju connection string i JWT podešavanja

### Zavisnosti (NuGet)

| Projekat | Ključni paketi |
|---|---|
| Domain | (bez eksternih paketa, samo `Shared.Domain`) |
| Application | `MediatR` , `FluentValidation` , `EFCore` |
| Infrastructure | `EFCore.SqlServer` , `EFCore.Tools` |
| API | `Grpc.AspNetCore` , `Microsoft.AspNetCore.Authentication.JwtBearer` , `Swashbuckle` |

---

## 7. Dijagram klasa

Dijagram klasa se nalazi u priloženom fajlu `Company-dijagram.drawio` unutar foldera `/docs/diagrams`.
