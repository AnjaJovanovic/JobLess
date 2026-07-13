# JobApplication Service

## Opis

Mikroservis za prijave kandidata na oglase za posao. Omogućava kreiranje prijave,
pregled prijava (kandidat / kompanija) i promenu statusa prijave (prihvatanje ili odbijanje).
Pri kreiranju prijave i promeni statusa šalju se poruke ka Notification servisu (MassTransit / RabbitMQ).

## Struktura

```
JobApplication/
├── JobLess.JobApplication.API
├── JobLess.JobApplication.Application
├── JobLess.JobApplication.Domain
└── JobLess.JobApplication.Infrastructure
```

## API Endpoints

| Metoda | Ruta | Autorizacija |
|--------|------|--------------|
| POST | `/api/job-applications` | Candidate – prijava na oglas |
| GET | `/api/job-applications/my` | Candidate – lista sopstvenih prijava |
| GET | `/api/job-applications/company` | Company – prijave (opcioni filter: oglas, status) |
| PATCH | `/api/job-applications/{id}/status` | Company – Accepted / Rejected |
| GET | `/api/job-applications/health` | bez autentifikacije |

### Status prijave

`Pending` (0) → `Accepted` (1) ili `Rejected` (2).  
Prelaz je dozvoljen samo iz `Pending`; ponovna promena statusa nije dozvoljena.

## Portovi

| Okruženje | Port |
|-----------|------|
| Lokalno | 5291 |
| Docker (host → kontejner) | 5291 → 8080 |

Swagger: http://localhost:5291/swagger

## Baza podataka

SQL Server, baza `JobLessJobApplicationDb`

### Tabela `JobApplications`

| Polje | Tip | Obavezno |
|-------|-----|----------|
| Id | int (PK, identity) | da |
| AdvertisementId | int | da |
| AdvertisementTitle | nvarchar(200) | da |
| CandidateId | int | da |
| CandidateEmail | nvarchar(256) | da |
| CandidateFirstName | nvarchar(100) | da |
| CandidateLastName | nvarchar(100) | da |
| CompanyId | int | da |
| CompanyEmail | nvarchar(256) | da |
| Status | int (`Pending` / `Accepted` / `Rejected`) | da |
| CreatedAt | datetime2 | da |
| UpdatedAt | datetime2 | ne |
| RowVersion | rowversion | da (sistemski, concurrency) |

Indeks: jedinstveni `(AdvertisementId, CandidateId)` — isti kandidat ne može dva puta na isti oglas.

Napomena: pri prijavi klijent šalje samo `AdvertisementId` i `CompanyId`; ostala obavezna polja se popunjavaju iz Client / Company / Advertisement lookup-a.

## Zavisnosti

Pri prijavi servis proverava:

| Servis | Protokol | Svrha |
|--------|----------|--------|
| Client | gRPC | postojanje profila kandidata |
| Company | gRPC | postojanje kompanije |
| Advertisement | HTTP | postojanje oglasa i pripadnost kompaniji |

### Eventi

| Poruka | Kada |
|--------|------|
| `JobAppliedMessage` | nova prijava |
| `ApplicationStatusChangedMessage` | accept / reject |

Potrebni su SQL Server i RabbitMQ. Za pun tok prijave moraju biti dostupni i Client, Company i Advertisement.

## Pokretanje

```bash
cd JobLess.JobApplication.API
dotnet run
```

Pokretanje celog stacka: `docker compose up` iz root foldera projekta.

## Dijagram

`docs/diagrams/klasni-dijagram-jobapplication.drawio`
