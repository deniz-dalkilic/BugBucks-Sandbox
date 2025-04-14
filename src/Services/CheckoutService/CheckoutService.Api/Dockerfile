FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/CheckoutService/CheckoutService.Api/CheckoutService.Api.csproj", "src/Services/CheckoutService/CheckoutService.Api/"]
RUN dotnet restore "src/Services/CheckoutService/CheckoutService.Api/CheckoutService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/CheckoutService/CheckoutService.Api"
RUN dotnet build "CheckoutService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CheckoutService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CheckoutService.Api.dll"]


# Use official .NET runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["CheckoutService.Api.csproj", "./"]
RUN dotnet restore "./CheckoutService.Api.csproj"
COPY . .
RUN dotnet build "CheckoutService.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CheckoutService.Api.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CheckoutService.Api.dll"]

