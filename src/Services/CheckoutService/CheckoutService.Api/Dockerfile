FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Services/CheckoutService/CheckoutService.Api/CheckoutService.Api.csproj", "src/Services/CheckoutService/CheckoutService.Api/"]
COPY ["src/Services/CheckoutService/CheckoutService.Application/CheckoutService.Application.csproj", "src/Services/CheckoutService/CheckoutService.Application/"]
COPY ["src/Services/CheckoutService/CheckoutService.Domain/CheckoutService.Domain.csproj", "src/Services/CheckoutService/CheckoutService.Domain/"]
COPY ["src/Shared/BugBucks.Shared.Messaging/BugBucks.Shared.Messaging.csproj", "src/Shared/BugBucks.Shared.Messaging/"]

RUN dotnet restore "src/Services/CheckoutService/CheckoutService.Api/CheckoutService.Api.csproj"

COPY . .
WORKDIR "/src/src/Services/CheckoutService/CheckoutService.Api"
RUN dotnet build "CheckoutService.Api.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/src/Services/CheckoutService/CheckoutService.Api"
RUN dotnet publish "CheckoutService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CheckoutService.Api.dll"]
