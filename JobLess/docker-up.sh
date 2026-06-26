#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")"

echo "Pokrećem JobLess stack (SQL + Auth + Client + Advertisement + Company + Frontend)..."
docker compose up --build -d

echo ""
echo "Servisi:"
echo "  Frontend:      http://localhost:5173"
echo "  Auth:          http://localhost:5218/swagger"
echo "  Client:        http://localhost:5263/swagger"
echo "  Advertisement: http://localhost:5104/swagger"
echo "  Company:       http://localhost:5287/swagger"
echo "  SQL Server:    localhost:1433"
echo ""
echo "Prati logove: docker compose logs -f"
echo "Zaustavi:      docker compose down"
