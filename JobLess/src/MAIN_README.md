# JobLess - Dokumentacija servisa

Web aplikacija za pretragu i prijavu na oglase za posao.
Dva tipa korisnika: **kandidat** i **kompanija**.

## Servisi

| Servis | Uloga |
|--------|--------|
| **Client** | Profil kandidata |
| **Company** | Profil kompanije |
| **Advertisement** | Kreiranje i pretraga oglasa |
| **JobApplication** | Prijava na oglas, accept/reject |
| **Notification** | Email i in-app notifikacije |
| **Identity Server** | Registracija, login, JWT |
| **Api Gateway** | Ulazna tačka (Ocelot) |

## Tok aplikacije

```
Frontend (React)
      |
      v
Api Gateway
      |
      +---> Identity Server
      +---> Client / Company / Advertisement
      +---> JobApplication
      +---> Notification
```

Interno: gRPC (lookup profila/oglasa), MassTransit event-i
(`UserRegistered`, `JobApplied`, `ApplicationStatusChanged`).

## Osnovni scenariji

1. Registracija → welcome email
2. Kandidat se prijavi na oglas → notifikacija kompaniji
3. Kompanija accept/reject → notifikacija kandidatu
