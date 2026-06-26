#!/usr/bin/env bash
set -euo pipefail

export DOTNET_ROOT="${DOTNET_ROOT:-$HOME/.dotnet}"
export DOTNET_MULTILEVEL_LOOKUP="${DOTNET_MULTILEVEL_LOOKUP:-1}"
export PATH="$DOTNET_ROOT:$PATH"
export ASPNETCORE_ENVIRONMENT="${ASPNETCORE_ENVIRONMENT:-Development}"

cd "$(dirname "$0")"

free_port() {
  local port=$1
  if command -v ss >/dev/null 2>&1 && ss -tln | grep -q ":${port} "; then
    echo "Port ${port} je zauzet. Zaustavljam stari proces..."
    fuser -k "${port}/tcp" 2>/dev/null || pkill -f 'JobLess.Advertisement.API' 2>/dev/null || true
    sleep 1
  fi
}

free_port 5104

dotnet run --project JobLess.Advertisement.API/JobLess.Advertisement.API.csproj --launch-profile http
