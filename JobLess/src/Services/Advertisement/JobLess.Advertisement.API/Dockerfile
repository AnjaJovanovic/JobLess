FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/ src/Shared/

COPY src/Services/Advertisement/JobLess.Advertisement.Domain/         src/Services/Advertisement/JobLess.Advertisement.Domain/
COPY src/Services/Advertisement/Jobless.Advertisement.Application/    src/Services/Advertisement/Jobless.Advertisement.Application/
COPY src/Services/Advertisement/JobLess.Advertisement.Infrastructure/ src/Services/Advertisement/JobLess.Advertisement.Infrastructure/
COPY src/Services/Advertisement/JobLess.Advertisement.API/            src/Services/Advertisement/JobLess.Advertisement.API/

RUN dotnet restore src/Services/Advertisement/JobLess.Advertisement.API/JobLess.Advertisement.API.csproj

RUN dotnet publish src/Services/Advertisement/JobLess.Advertisement.API/JobLess.Advertisement.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.Advertisement.API.dll"]
