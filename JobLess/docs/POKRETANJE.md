# Uputstvo za pokretanje

Ovaj dokument opisuje kako se JobLess sistem podešava i pokreće lokalno, u razvojnom okruženju. Projekat nije javno objavljen (deployovan) na spoljnom serveru — pokreće se lokalno putem Dockera.

## Preduslovi

| Alat | Verzija | Potrebno za |
|---|---|---|
| [Docker](https://www.docker.com/) + Docker Compose | najnovija stabilna | pokretanje kompletnog sistema (preporučeno) |
| [.NET SDK](https://dotnet.microsoft.com/) | 8.0.x | pokretanje pojedinačnog servisa van Dockera, `dotnet test` |
| [Node.js](https://nodejs.org/) | 18+ | pokretanje frontenda u dev modu (`npm run dev`) |
| SQL klijent (opciono) | — | inspekcija baze (`localhost:1433`) |

Verzija .NET SDK-a je fiksirana u [`global.json`](../JobLess/global.json) na `8.0.*`.

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

### Redosled pokretanja i migracije

`docker-compose.yml` definiše `depends_on` sa `condition: service_healthy` za SQL Server i RabbitMQ, tako da se servisi ne pokreću dok baza i broker nisu spremni. Svaki servis pri startu sam primenjuje EF Core migracije (`db.Database.Migrate()` u `Program.cs`), pa nije potrebna ručna inicijalizacija baze.

### Konfiguracija (environment promenljive)

Sve promenljive za Development okruženje su već podešene direktno u `docker-compose.yml` (connection stringovi, JWT ključ, RabbitMQ i SMTP kredencijali). Za realan projekat ove vrednosti bi trebalo da budu u `.env` fajlu ili secret manageru — ovde su hardkodovane radi jednostavnosti pokretanja u okviru seminarskog rada.

Najvažnije promenljive po servisu:

- `ConnectionStrings__DefaultConnection` / `ConnectionStrings__IdentityConnectionString` — konekcija ka SQL Serveru
- `Jwt__Key`, `Jwt__Issuer`, `Jwt__Audience`, `Jwt__ExpirationMinutes` — moraju biti **identični** u Auth servisu i u svakom servisu koji validira JWT (Client, Company, Notification, JobApplication)
- `RabbitMq__Host`, `RabbitMq__Username`, `RabbitMq__Password` — konekcija ka RabbitMQ-u
- `Smtp__*` (samo Notification servis) — SMTP nalog za slanje email obaveštenja

> **Napomena:** JWT ključ i SMTP lozinka u `docker-compose.yml` su primer/demo vrednosti namenjene isključivo lokalnom razvoju za potrebe seminarskog rada i ne treba ih koristiti u produkcionom okruženju.

## Opcija 2 — Pokretanje pojedinačnih servisa lokalno (bez Dockera)

Koristi se kada se razvija/debaguje jedan servis, dok ostali rade u Dockeru (ili takođe lokalno).

1. Pokrenuti samo infrastrukturu (SQL Server + RabbitMQ) iz Docker Compose-a:

   ```bash
   docker compose up -d sql-server rabbitmq
   ```

2. Pokrenuti željeni servis direktno:

   ```bash
   cd src/Security/JobLess.IdentityServer.API
   dotnet run
   ```

   Servis će koristiti `appsettings.Development.json` / `launchSettings.json` iz svog projekta (portovi se poklapaju sa onima iz tabele iznad, npr. Auth na `5218`).

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
