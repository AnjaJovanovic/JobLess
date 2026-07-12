FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/ApiGateway/JobLess.ApiGateway.API/ src/ApiGateway/JobLess.ApiGateway.API/

RUN dotnet restore src/ApiGateway/JobLess.ApiGateway.API/JobLess.ApiGateway.API.csproj

RUN dotnet publish src/ApiGateway/JobLess.ApiGateway.API/JobLess.ApiGateway.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "JobLess.ApiGateway.API.dll"]
