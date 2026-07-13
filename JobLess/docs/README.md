# JobLess korisničko uputstvo

JobLess je web aplikacija za oglase za posao i prijave. Postoje dva tipa naloga: **kandidat** i **kompanija**.

Aplikacija se otvara u pregledaču na adresi **http://localhost:5173** (kad je frontend pokrenut). Prijava je na stranici `/login`.

## 1. Registracija i prijava

Na ekranu za prijavu bira se tip naloga: kandidat ili kompanija. Može se prelaziti između **Prijave** i **Registracije**.

### 1.1 Kandidat registracija

Potrebna polja:

- ime i prezime
- email
- lozinka (najmanje 8 karaktera, bar jedno veliko slovo i bar jedan broj) i potvrda lozinke
- pol
- telefon (opciono; ako se unese, format je npr. `+381 60 123 4567`)

Posle uspešne registracije sledi automatska prijava i prelazak na kontrolnu tablu kandidata.

### 1.2 Kompanija registracija

Potrebna polja uključuju:

- naziv kompanije
- PIB (9 cifara) i matični broj (8 cifara)
- email, telefon
- industrija, broj zaposlenih, grad
- ime i prezime kontakt osobe i pozicija
- lozinka i potvrda lozinke
- sajt (opciono)

### 1.3 Prijava

Unose se email i lozinka za izabrani tip naloga. Posle uspešne prijave:

- kandidat ide na `/user`
- kompanija ide na `/company`

Ako sesija istekne ili nalog nije prijavljen, stranice zaštite vraćaju na `/login`.

### 1.4 Odjava

U bočnom meniju postoji dugme **Odjavi se**. Posle odjave ponovo se otvara ekran za prijavu.

## 2. Nalog kandidata

Bočni meni:

| Stavka | Sadržaj |
|--------|---------|
| Moj profil | Pregled i uređivanje profila |
| Moje prijave | Lista prijava na oglase i statusi |
| Oglasi | Pretraga oglasa i prijava |
| Obaveštenja | Poruke (npr. promena statusa prijave) |

Broj nepročitanih obaveštenja prikazuje se kao bedž pored stavke **Obaveštenja**.

### 2.1 Moj profil

Profil treba biti popunjen pre prijave na oglas. Za kompletan profil potrebni su bar:

- ime i prezime
- pol
- nivo obrazovanja
- naziv institucije

Ostala polja (telefon, datum rođenja, grad, adresa, godine obrazovanja, godine iskustva, veštine, kratak opis, LinkedIn) su opciona, ali pomažu pri pregledu od strane kompanije.

Mogućnosti:

- prvo popunjavanje profila (setup)
- pregled sačuvanog profila
- izmena postojećih podataka

### 2.2 Oglasi

Otvara se lista aktivnih oglasa. Dostupni su filteri (npr. naslov, grad, tip rada, senioritet, vrsta zaposlenja, radno vreme, iskustvo, plata).

Prijava na oglas:

1. Profil mora biti kompletan (inače prijava nije dozvoljena).
2. Na oglasu se bira akcija prijave.
3. Jedan kandidat ne može dva puta da se prijavi na isti oglas.

### 2.3 Moje prijave

Prikazuje se lista prijava sa statusom:

| Status | Značenje |
|--------|----------|
| U razmatranju | Prijava čeka odluku kompanije |
| Prihvaćen | Kompanija je prihvatila prijavu |
| Odbijen | Kompanija je odbila prijavu |

### 2.4 Obaveštenja (kandidat)

Pojavljuju se obaveštenja o promeni statusa prijave (prihvatanje ili odbijanje). Obaveštenje se može otvoriti radi detalja i označiti kao pročitano.

## 3. Nalog kompanije

Bočni meni:

| Stavka | Sadržaj |
|--------|---------|
| Profil kompanije | Podaci o kompaniji |
| Kreiraj oglas | Novi oglas za posao |
| Moji oglasi | Pregled i upravljanje oglasima |
| Prijave | Prijave kandidata na oglase |
| Obaveštenja | Nove prijave i slične poruke |

### 3.1 Profil kompanije

Pregled i ažuriranje podataka o kompaniji (kontakt, adresa, opis i slično, u zavisnosti od forme u aplikaciji).

### 3.2 Kreiranje oglasa

Unose se podaci oglasa, npr.:

- naslov i pozicija
- opis
- tip zaposlenja, radno vreme, senioritet
- iskustvo (min/max)
- lokacija, tip rada
- plata i valuta (po potrebi)

Posle uspešnog kreiranja otvara se lista oglasa (**Moji oglasi**).

### 3.3 Moji oglasi

Pregled oglasa koje je kompanija objavila. Odavde se može preći na kreiranje novog oglasa.

### 3.4 Prijave

Lista kandidata koji su se prijavili. Filteri:

- po ID-u oglasa
- po statusu (svi / u razmatranju / prihvaćen / odbijen)

Za svaku prijavu u statusu **U razmatranju** moguće je:

- **prihvatanje** prijave
- **odbijanje** prijave

Status se menja samo jednom (iz "u razmatranju"); kasnija promena nije dozvoljena.

Iz liste se može otvoriti profil kandidata radi detaljnijeg pregleda.

### 3.5 Obaveštenja (kompanija)

Stiže obaveštenje kada se kandidat prijavi na oglas. Iz obaveštenja se mogu otvoriti detalji (npr. kandidat / oglas) i označiti pročitano.

## 4. Tipičan tok korišćenja

1. Kompanija se registruje, popuni profil i objavi oglas.
2. Kandidat se registruje, popuni profil i nađe oglas (filteri po želji).
3. Kandidat šalje prijavu.
4. Kompanija dobija obaveštenje, pregleda prijavu i profil kandidata, pa prihvata ili odbija.
5. Kandidat dobija obaveštenje o novom statusu i vidi ga u **Moje prijave**.

## 5. Napomene i česti problemi

- **Prijava ne radi bez kompletnog profila:** dopuniti obrazovanje (nivo + institucija), ime, prezime i pol.
- **Telefon** mora biti u međunarodnom formatu ako se unosi (`+381 60 123 4567`).
- **Lozinka** mora zadovoljiti pravila dužine i sastava (veliko slovo + broj).
- Ako lista oglasa ili prijava ne učitava podatke, proveriti da li je backend (Api Gateway i servisi) pokrenut; frontend proksira `/api` ka gateway-u na `http://localhost:5000`.
- Aplikacija zahteva prijavljen nalog za sve stranice osim `/login`.

## 6. Ukratko o ulogama

| Uloga | Može |
|-------|------|
| Kandidat | Profil, pregled oglasa, prijava, praćenje statusa, obaveštenja |
| Kompanija | Profil, oglasi, pregled prijava, accept/reject, obaveštenja |

JobLess ne zamenjuje email komunikaciju van aplikacije; sve ključne radnje oko prijave i statusa odvijaju se kroz kontrolnu tablu i obaveštenja u aplikaciji.
