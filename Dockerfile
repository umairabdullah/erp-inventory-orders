# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Project files first, so the restore layer is cached until a package changes
COPY Src/Core/Erp.Domain/Erp.Domain.csproj Src/Core/Erp.Domain/
COPY Src/Core/Erp.Application/Erp.Application.csproj Src/Core/Erp.Application/
COPY Src/Infrastructure/Erp.Infrastructure/Erp.Infrastructure.csproj Src/Infrastructure/Erp.Infrastructure/
COPY Src/Api/Erp.Api/Erp.Api.csproj Src/Api/Erp.Api/
RUN dotnet restore Src/Api/Erp.Api/Erp.Api.csproj

COPY Src/ Src/
# Remove Windows build output copied from the host, then restore again inside Linux
RUN find Src -type d \( -name bin -o -name obj \) -prune -exec rm -rf {} + \
 && dotnet restore Src/Api/Erp.Api/Erp.Api.csproj
RUN dotnet publish Src/Api/Erp.Api/Erp.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN mkdir -p /app/logs && chown -R $APP_UID /app/logs
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Erp.Api.dll"]
