# Client Service

## Opis

Mikroservis za upravljanje profilima kandidata. Čuva lične podatke, obrazovanje i radno iskustvo.
Profil je dostupan kandidatu (sopstveni nalog) i kompaniji (pregled kandidata nakon prijave).
Ostali servisi koriste gRPC lookup za osnovne podatke o kandidatu.

## Struktura

```
Client/
├── JobLess.Client.API
├── JobLess.Client.Application
├── JobLess.Client.Domain
└── JobLess.Client.Infrastructure
```

## API Endpoints

| Metoda | Ruta | Autorizacija |
|--------|------|--------------|
| PUT | `/api/clients/profile` | Candidate – kreiranje profila |
| GET | `/api/clients/profile/by-email?email=` | Candidate – profil po emailu |
| PUT | `/api/clients/{id}/profile` | Candidate – ažuriranje profila |
| GET | `/api/clients/{id}/profile` | Candidate, Company – profil po ID-u |

## Portovi

| Okruženje | HTTP | gRPC |
|-----------|------|------|
| Lokalno | 5263 | 5264 |
| Docker (host → kontejner) | 5263 → 8080 | 5264 → 8081 |

Swagger: http://localhost:5263/swagger

## Baza podataka

SQL Server, baza `JobLessClientDb`

### Tabela `Clients`

| Polje | Tip | Obavezno |
|-------|-----|----------|
| ClientId | int (PK, identity) | da |
| Email | nvarchar | da |
| PasswordHash | nvarchar | da |
| FirstName | nvarchar | da |
| LastName | nvarchar | da |
| PhoneNumber | nvarchar | ne |
| Gender | int (`Male` / `Female`) | da |
| DateOfBirth | datetime2 | ne |
| City | nvarchar | ne |
| Address | nvarchar | ne |
| EducationLevel | int (enum) | ne |
| InstitutionName | nvarchar | ne |
| EducationStartYear | int | ne |
| EducationEndYear | int | ne |
| YearsOfExperience | int | ne |
| Skills | nvarchar | ne |
| ProfessionalSummary | nvarchar | ne |
| LinkedInUrl | nvarchar | ne |
| CreatedAt | datetime2 | da |
| IsActive | bit | da |

Napomena: pri kreiranju profila preko API-ja `PasswordHash` se čuva kao prazan string (lozinka je u Identity servisu). `Email` dolazi iz JWT tokena, ne iz tela zahteva.

## Integracija

- gRPC endpoint za lookup profila (koristi JobApplication i slični servisi)
- JWT autentifikacija

## Pokretanje

```bash
cd JobLess.Client.API
dotnet run
```

Pokretanje celog stacka: `docker compose up` iz root foldera projekta.

## Dijagram

`docs/diagrams/klasni-dijagram-client.drawio`
