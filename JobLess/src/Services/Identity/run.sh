#!/usr/bin/env bash
set -euo pipefail

export DOTNET_ROOT="${DOTNET_ROOT:-/usr/share/dotnet}"
export DOTNET_MULTILEVEL_LOOKUP="${DOTNET_MULTILEVEL_LOOKUP:-1}"
export PATH="$DOTNET_ROOT:$PATH"
export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"

cd "$(dirname "$0")"

if command -v ss >/dev/null 2>&1 && ss -tln | grep -q ':5263 '; then
  echo "Port 5263 je zauzet. Zaustavljam stari JobLess.Identity.API..."
  pkill -f 'JobLess.Identity.API' 2>/dev/null || true
  sleep 1
fi

dotnet run --project JobLess.Identity.API.csproj --launch-profile http
