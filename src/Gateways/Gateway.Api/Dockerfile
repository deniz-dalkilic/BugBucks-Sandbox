FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Gateways/Gateway.Api/Gateway.Api.csproj", "src/Gateways/Gateway.Api/"]
COPY ["src/Shared/BugBucks.Shared.Vault/BugBucks.Shared.Vault.csproj", "src/Shared/BugBucks.Shared.Vault/"]
COPY ["src/Shared/BugBucks.Shared.Logging/BugBucks.Shared.Logging.csproj", "src/Shared/BugBucks.Shared.Logging/"]
RUN dotnet restore "src/Gateways/Gateway.Api/Gateway.Api.csproj"
COPY . .
WORKDIR "/src/src/Gateways/Gateway.Api"
RUN dotnet build "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
