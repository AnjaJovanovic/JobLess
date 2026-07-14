# Uputstvo za pokretanje

Ovaj dokument opisuje kako se JobLess sistem podešava i pokreće lokalno, u razvojnom okruženju. Projekat nije javno objavljen (deployovan) na spoljnom serveru — pokreće se lokalno putem Dockera.

## Konfiguracija (environment promenljive)

Aplikacija koristi `.env` fajl za konfiguraciju osetljivih parametara kao što su lozinke za baze podataka, JWT ključevi, SMTP i RabbitMQ kredencijali. Iz bezbednosnih razloga, `.env` fajl se ne nalazi na Git repozitorijumu.

**Kopirati primer konfiguracije:**

   Napraviti kopiju `.env.example` fajla i nazivati ga `.env`, pokretanjem sledeće komande u terminalu (iz korena projekta):

   ```bash
   cp .env.example .env # Za Linux i macOS
   copy .env.example .env # Za Windows Command Prompt
   ```
**Popuniti vrednosti u .env fajlu**

   Otvoriti novokreirani .env fajl i popuniti prazne promenljive (poput SQL_SA_PASSWORD, JWT_KEY, RABBITMQ_PASSWORD, itd.).


## Pokretanje kompletnog sistema (Docker Compose)

Ovo je preporučen način pokretanja jer podiže sve mikroservise, bazu, RabbitMQ, gateway i frontend odjednom, sa ispravno podešenom mrežnom komunikacijom između kontejnera.

```bash
cd JobLess/JobLess
./docker-up.sh
```

Skripta interno poziva `docker compose up --build -d`. Prvo pokretanje traje duže jer se prave svi image-i i preuzima SQL Server image.

Nakon pokretanja dostupno je:

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
| RabbitMQ Management UI | http://localhost:15672 (guest / guest) |
| SQL Server | `localhost,1433` (sa / `sa` / `JobLess_Pass123!`) |

Korisne komande:

```bash
docker compose logs -f              # praćenje logova svih servisa
docker compose logs -f auth-service # logovi samo jednog servisa
docker compose down                 # zaustavljanje (podaci u volume-u ostaju)
docker compose down -v              # zaustavljanje + brisanje baze (čist start)
```



## Opcija 2 — Pokretanje pojedinačnih servisa lokalno (bez Dockera)

Koristi se kada se razvija/debaguje jedan servis, dok ostali rade u Dockeru (ili takođe lokalno).

1. Pokrenuti samo infrastrukturu (SQL Server + RabbitMQ) iz Docker Compose-a:

   ```bash
   docker compose up -d sql-server rabbitmq
   ```
2. Podesiti user-secrets za servis koji se pokreće

Pokrenuti jednom po servisu, iz foldera`*.API` projekta tog servisa. Vrednosti uzeti iz `.env` fajla (isti ključ/lozinka).

**Auth (Security):**
```bash
cd src/Security/JobLess.IdentityServer.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:IdentityConnectionString" "Server=localhost,1433;Database=JobLessIdentityDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "jobless"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
```

**Client / Company / Advertisement / JobApplication (isti obrazac):**
```bash
cd src/Services//JobLess..API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JobLessDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "jobless"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
```

**Notification (dodatno ima SMTP):**
```bash
cd src/Services/Notification/JobLess.Notification.API
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "JWT_KEY_VREDNOST"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=JobLessNotificationDb;User Id=sa;Password=SA_PASSWORD_VREDNOST;TrustServerCertificate=True;MultipleActiveResultSets=True;"
dotnet user-secrets set "RabbitMq:Username" "jobless"
dotnet user-secrets set "RabbitMq:Password" "RABBITMQ_PASSWORD_VREDNOST"
dotnet user-secrets set "Smtp:UserName" "jobless.matf@gmail.com"
dotnet user-secrets set "Smtp:Password" "SMTP_PASSWORD_VREDNOST"
dotnet user-secrets set "Smtp:FromEmail" "jobless.matf@gmail.com"
```

> **Napomena o sintaksi:** u `.env`/`docker-compose.yml` ključevi koriste `__`
> (npr. `Jwt__Key`), u `dotnet user-secrets` koriste `:` (npr. `Jwt:Key`) — ista
> promenljiva, druga notacija.

Proveri šta je upisano:
```bash
dotnet user-secrets list
```

3. Pokrenuti željeni servis direktno:

   ```bash
   cd src/Security/JobLess.IdentityServer.API
   dotnet run
   ```

3. Za API Gateway u ovom režimu koristiti profil koji rutira ka `localhost` (umesto ka Docker imenima kontejnera) — pokrenuti sa `ASPNETCORE_ENVIRONMENT=Local`, kako bi se učitao `ocelot.Local.json` (rute ka `localhost:<port>` za svaki servis). Podrazumevani `ASPNETCORE_ENVIRONMENT=Development` učitava `ocelot.Development.json`, koji rutira ka imenima Docker kontejnera (npr. `client-service`) i **neće raditi** ako servisi nisu u istoj Docker mreži.

   ```bash
   cd src/ApiGateway/JobLess.ApiGateway.API
   ASPNETCORE_ENVIRONMENT=Local dotnet run
   ```

## Pokretanje frontenda u dev modu

```bash
cd src/frontend
npm install
npm run dev
```

Vite dev server po defaultu radi na `http://localhost:5173` i (u Docker okruženju) šalje sve `/api/*` pozive ka istom originu; kada se frontend pokreće izvan Dockera, ovi pozivi moraju da idu ka pokrenutom API Gateway-u (proveriti `vite.config.js` / proxy podešavanje ili koristiti apsolutne URL-ove ka `http://localhost:5000`).

## Pokretanje testova

```bash
cd JobLess
dotnet test
```

Pokreće sve xUnit test projekte iz `src/Tests/` (uključujući `JobLess.Tests.Security` i `JobLess.Tests.Notification`).

## Rešavanje uobičajenih problema

| Problem | Uzrok / rešenje |
|---|---|
| Servisi se gase odmah po startu, `depends_on` čeka unedogled | SQL Server healthcheck-u treba do ~30s pri prvom podizanju (`start_period: 30s`) — sačekati ili proveriti `docker compose logs sql-server`. |
| `401 Unauthorized` na zaštićenim endpoint-ima | JWT `Key`/`Issuer`/`Audience` moraju biti identični u Auth servisu i servisu koji poziva se. Proveriti da token nije istekao (podrazumevano `5` minuta, vidi `Jwt__ExpirationMinutes`). |
