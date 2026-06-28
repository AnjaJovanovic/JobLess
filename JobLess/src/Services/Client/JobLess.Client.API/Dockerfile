FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/JobLess.Contracts/                      src/Shared/JobLess.Contracts/
COPY src/Services/Client/JobLess.Client.Domain/         src/Services/Client/JobLess.Client.Domain/
COPY src/Services/Client/JobLess.Client.Application/    src/Services/Client/JobLess.Client.Application/
COPY src/Services/Client/JobLess.Client.Infrastructure/ src/Services/Client/JobLess.Client.Infrastructure/
COPY src/Services/Client/JobLess.Client.API/            src/Services/Client/JobLess.Client.API/

RUN dotnet restore src/Services/Client/JobLess.Client.API/JobLess.Client.API.csproj

RUN dotnet publish src/Services/Client/JobLess.Client.API/JobLess.Client.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.Client.API.dll"]
