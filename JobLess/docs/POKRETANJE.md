# Uputstvo za pokretanje

Ovaj dokument opisuje kako se JobLess sistem podešava i pokreće lokalno, u razvojnom okruženju. Projekat nije javno objavljen (deployovan) na spoljnom serveru — pokreće se lokalno putem Dockera.

## Konfiguracija (environment promenljive)

Aplikacija koristi `.env` fajl za konfiguraciju osetljivih parametara kao što su lozinke za baze podataka, JWT ključevi, SMTP i RabbitMQ kredencijali. Iz bezbednosnih razloga, `.env` fajl se ne nalazi na Git repozitorijumu.

Svi ključevi moraju odgovarati imenima koja `docker-compose.yml` očekuje.

### Kopirati primer konfiguracije

Raditi iz foldera `JobLess/JobLess` (gde su `docker-compose.yml` i `.env.example`).

**Linux / macOS:**

```bash
cp .env.example .env
```

**Windows — Command Prompt (cmd):**

```bat
copy .env.example .env
```

**Windows — PowerShell:**

```powershell
Copy-Item .env.example .env
```

### Popuniti vrednosti u `.env` fajlu

Otvoriti novokreirani `.env` i popuniti prazne promenljive:

| Ključ | Opis |
|---|---|
| `SA_PASSWORD` | Lozinka za SQL Server korisnika `sa` |
| `JWT_KEY` | Tajni ključ za potpisivanje JWT-a (dovoljno dug, npr. 32+ karaktera) |
| `JWT_ISSUER` / `JWT_AUDIENCE` | Issuer i audience (isti u svim servisima) |
| `JWT_EXPIRATION_MINUTES` | Trajanje access tokena u minutima |
| `RABBITMQ_USER` / `RABBITMQ_PASSWORD` | Kredencijali RabbitMQ-a |
| `SMTP_*` | Podaci za slanje email-a (Notification servis) |


## Pokretanje kompletnog sistema (Docker Compose)

Ovo je preporučen način pokretanja jer podiže sve mikroservise, bazu, RabbitMQ, gateway i frontend odjednom, sa ispravno podešenom mrežnom komunikacijom između kontejnera.

Prvo pokretanje traje duže jer se grade image-i i preuzima SQL Server image.

### Linux / macOS

```bash
cd JobLess/JobLess
./docker-up.sh
```

Skripta interno poziva `docker compose up --build -d`. Ekvivalentno direktno:

```bash
docker compose up --build -d
```

### Windows (PowerShell ili cmd)

`docker-up.sh` je bash skripta i **ne pokreće se** iz običnog cmd/PowerShell-a. Koristiti Compose direktno:

```bat
cd JobLess\JobLess
docker compose up --build -d
```

### Adrese posle pokretanja

| Servis | URL |
|---|---|
| Frontend | http://localhost:5173 |
| API Gateway | http://localhost:5000 |
| Auth (Swagger) | http://localhost:5218/swagger |
| Client (Swagger) | http://localhost:5263/swagger |
| Advertisement (Swagger) | http://localhost:5104/swagger |
| Company (Swagger) | http://localhost:5287/swagger |
| Notification (Swagger) | http://localhost:5240/swagger |
| JobApplication (Swagger) | http://localhost:5291/swagger |

### Korisne komande

```bash
docker compose logs -f              # praćenje logova svih servisa
docker compose logs -f auth-service # logovi samo jednog servisa
docker compose down                 # zaustavljanje (podaci u volume-u ostaju)
docker compose down -v              # zaustavljanje + brisanje baze (čist start)
```
## Opcija 2 — Pokretanje pojedinačnih servisa lokalno (bez Dockera)

Koristi se kada se razvija/debaguje jedan servis, dok ostali rade u Dockeru (ili takođe lokalno). Potrebni su .NET 8 SDK i (za frontend) Node.js.

1. Pokrenuti samo infrastrukturu (SQL Server + RabbitMQ) iz Docker Compose-a:

   ```bash
   docker compose up -d sql-server rabbitmq
   ```

2. Podesiti user-secrets za servis koji se pokreće.

Pokrenuti jednom po servisu, iz foldera `*.API` projekta tog servisa. Vrednosti uzeti iz `.env` fajla (isti ključ/lozinka).

**Auth (Security):**

```bash
cd src/Security/JobLess.IdentityServer.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:IdentityConnectionString" "Server=localhost,1433;Database=JobLessIdentityDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "RABBITMQ_USER_VREDNOST"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
```

**Client / Company / Advertisement / JobApplication** (isti obrazac; zameniti putanju i ime baze po servisu):

```bash
cd src/Services/Client/JobLess.Client.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JobLessClientDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "RABBITMQ_USER_VREDNOST"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
```

**Notification** (dodatno ima SMTP):

```bash
cd src/Services/Notification/JobLess.Notification.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JobLessNotificationDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "RABBITMQ_USER_VREDNOST"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
dotnet user-secrets set "Smtp:UserName" "SMTP_USERNAME_VREDNOST"
dotnet user-secrets set "Smtp:Password" "SMTP_PASSWORD_VREDNOST"
dotnet user-secrets set "Smtp:FromEmail" "SMTP_FROM_EMAIL_VREDNOST"
```

> **Napomena o sintaksi:** u `.env` / `docker-compose.yml` okruženju servisi koriste `__` (npr. `Jwt__Key`), u `dotnet user-secrets` koriste `:` (npr. `Jwt:Key`) — ista promenljiva, druga notacija.

Proveri šta je upisano:

```bash
dotnet user-secrets list
```

3. Pokrenuti željeni servis direktno:

```bash
cd src/Security/JobLess.IdentityServer.API
dotnet run
```

## Pokretanje frontenda u dev modu

```bash
cd src/frontend
npm install
npm run dev
```

Na Windowsu iste komande u PowerShell ili cmd (putanja: `src\frontend`).

Vite dev server po defaultu radi na `http://localhost:5173`. Kada se frontend pokreće izvan Dockera, `/api/*` pozivi moraju da idu ka API Gateway-u (`http://localhost:5000`) — proveriti `vite.config.js` / proxy.

## Pokretanje testova

```bash
cd JobLess/JobLess
dotnet test
```

Pokreće sve xUnit test projekte iz `src/Tests/`.

## Rešavanje uobičajenih problema

| Problem | Uzrok / rešenje |
|---|---|
| Servisi se gase odmah po startu, `depends_on` čeka unedogled | SQL Server healthcheck-u treba do ~30s pri prvom podizanju (`start_period: 30s`) — sačekati ili proveriti `docker compose logs sql-server`. |
| `401 Unauthorized` na zaštićenim endpoint-ima | JWT `Key`/`Issuer`/`Audience` moraju biti identični u Auth servisu i servisu koji validira token. Proveriti da token nije istekao (`JWT_EXPIRATION_MINUTES` u `.env`). |
| Compose ne vidi varijable / prazan `SA_PASSWORD` | Proveriti da je fajl `.env` (ne `.env.example`) u `JobLess/JobLess` i da se ključ zove `SA_PASSWORD`. |
| Windows: `./docker-up.sh` ne radi | Očekivano u cmd/PowerShell — koristiti `docker compose up --build -d`. |
| Windows: fajl `.env.txt` | Explorer je dodao ekstenziju — preimenovati u `.env`. |
| Windows: Docker daemon not running | Pokrenuti Docker Desktop i sačekati *Engine running*. |
| Windows: spor SQL image / greške WSL | U Docker Desktop Settings proveriti WSL 2 backend i dovoljno RAM-a za WSL. |
| RabbitMQ login ne uspeva sa `guest`/`guest` | Podrazumevani korisnik nije `guest` — koristiti `RABBITMQ_USER` / `RABBITMQ_PASSWORD` iz `.env`. |
