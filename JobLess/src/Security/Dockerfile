
# must be run from the JobLess/ solution root
#   docker build -f src/Security/Dockerfile -t jobless-auth .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy shared domain first (referenced by auth projects)
COPY src/Shared/ src/Shared/

COPY src/Security/JobLess.IdentityServer.Domain/         src/Security/JobLess.IdentityServer.Domain/
COPY src/Security/JobLess.IdentityServer.Application/    src/Security/JobLess.IdentityServer.Application/
COPY src/Security/JobLess.IdentityServer.Infrastructure/ src/Security/JobLess.IdentityServer.Infrastructure/
COPY src/Security/JobLess.IdentityServer.API/            src/Security/JobLess.IdentityServer.API/

RUN dotnet restore src/Security/JobLess.IdentityServer.API/JobLess.IdentityServer.API.csproj

RUN dotnet publish src/Security/JobLess.IdentityServer.API/JobLess.IdentityServer.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.IdentityServer.API.dll"]
