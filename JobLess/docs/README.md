# JobLess Dokumentacija

Tehnička dokumentacija backend i frontend dela projekta.

## Backend (Doxygen)

- Lokacija: `doxygen/html/index.html`
- Alat: Doxygen + Graphviz

```bash
# iz root-a JobLess/
doxygen Doxyfile
```

## Frontend (JSDoc)

- Lokacija: `frontend/index.html`
- Alat: JSDoc (React frontend je u JavaScriptu / JSX)

```bash
cd src/frontend
npm run docs:frontend
```

## Generisanje obe dokumentacije

```bash
cd src/frontend
npm run docs:all
```

## Pregled u browseru

```bash
cd src/frontend
npm run docs:serve
```

- Backend: http://localhost:8000/doxygen/html/
- Frontend: http://localhost:8000/frontend/

## Dijagrami

Klasni dijagrami (draw.io / diagrams.net):

- `diagrams/klasni-dijagram-client.drawio`
- `diagrams/klasni-dijagram-jobapplication.drawio`

## Dokumentovanje koda

- C#: XML komentari (`///`)
- JS/JSX: JSDoc (`/** */`)
