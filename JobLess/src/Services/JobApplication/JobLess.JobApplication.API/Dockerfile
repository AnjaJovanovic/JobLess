FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Services/JobApplication/JobLess.JobApplication.Domain/         src/Services/JobApplication/JobLess.JobApplication.Domain/
COPY src/Services/JobApplication/JobLess.JobApplication.Application/    src/Services/JobApplication/JobLess.JobApplication.Application/
COPY src/Services/JobApplication/JobLess.JobApplication.Infrastructure/ src/Services/JobApplication/JobLess.JobApplication.Infrastructure/
COPY src/Services/JobApplication/JobLess.JobApplication.API/            src/Services/JobApplication/JobLess.JobApplication.API/

RUN dotnet restore src/Services/JobApplication/JobLess.JobApplication.API/JobLess.JobApplication.API.csproj

RUN dotnet publish src/Services/JobApplication/JobLess.JobApplication.API/JobLess.JobApplication.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.JobApplication.API.dll"]
