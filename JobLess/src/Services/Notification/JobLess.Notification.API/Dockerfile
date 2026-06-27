# must be run from the JobLess/ solution root
#   docker build -f src/Services/Notification/JobLess.Notification.API/Dockerfile -t jobless-notification .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Shared/ src/Shared/

COPY src/Services/Notification/JobLess.Notification.Domain/         src/Services/Notification/JobLess.Notification.Domain/
COPY src/Services/Notification/JobLess.Notification.Application/    src/Services/Notification/JobLess.Notification.Application/
COPY src/Services/Notification/JobLess.Notification.Infrastructure/ src/Services/Notification/JobLess.Notification.Infrastructure/
COPY src/Services/Notification/JobLess.Notification.API/            src/Services/Notification/JobLess.Notification.API/

RUN dotnet restore src/Services/Notification/JobLess.Notification.API/JobLess.Notification.API.csproj

RUN dotnet publish src/Services/Notification/JobLess.Notification.API/JobLess.Notification.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.Notification.API.dll"]
