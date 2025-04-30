FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/IdentityService/IdentityService.Api/IdentityService.Api.csproj", "src/Services/IdentityService/IdentityService.Api/"]
COPY ["src/Shared/BugBucks.Shared.VaultClient/BugBucks.Shared.VaultClient.csproj", "src/Shared/BugBucks.Shared.VaultClient/"]
COPY ["src/Services/IdentityService/IdentityService.Infrastructure/IdentityService.Infrastructure.csproj", "src/Services/IdentityService/IdentityService.Infrastructure/"]
COPY ["src/Services/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "src/Services/IdentityService/IdentityService.Application/"]
COPY ["src/Services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "src/Services/IdentityService/IdentityService.Domain/"]
COPY ["src/Shared/BugBucks.Shared.Logging/BugBucks.Shared.Logging.csproj", "src/Shared/BugBucks.Shared.Logging/"]
RUN dotnet restore "src/Services/IdentityService/IdentityService.Api/IdentityService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/IdentityService/IdentityService.Api"
RUN dotnet build "IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]
