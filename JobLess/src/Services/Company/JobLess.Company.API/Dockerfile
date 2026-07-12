FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/JobLess.Grpc.Contracts/ src/Shared/JobLess.Grpc.Contracts/
COPY src/Shared/JobLess.Shared.Domain/ src/Shared/JobLess.Shared.Domain/

COPY src/Services/Company/JobLess.Company.Domain/         src/Services/Company/JobLess.Company.Domain/
COPY src/Services/Company/JobLess.Company.Application/    src/Services/Company/JobLess.Company.Application/
COPY src/Services/Company/JobLess.Company.Infrastructure/ src/Services/Company/JobLess.Company.Infrastructure/
COPY src/Services/Company/JobLess.Company.API/            src/Services/Company/JobLess.Company.API/

RUN dotnet restore src/Services/Company/JobLess.Company.API/JobLess.Company.API.csproj

RUN dotnet publish src/Services/Company/JobLess.Company.API/JobLess.Company.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "JobLess.Company.API.dll"]
