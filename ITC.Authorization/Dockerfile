FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ITC.Authorization/ITC.Authorization.csproj", "ITC.Authorization/"]
COPY ["ITC.Domain/ITC.Domain.csproj", "ITC.Domain/"]
COPY ["ITC.CQRS/ITC.CQRS.csproj", "ITC.CQRS/"]
COPY ["ITC.Storage/ITC.Storage.csproj", "ITC.Storage/"]
COPY ["ITC.ServiceBus/ITC.ServiceBus.csproj", "ITC.ServiceBus/"]
COPY ["Keycloak.Net.Core/Keycloak.Net.Core.csproj", "Keycloak.Net.Core/"]
RUN dotnet restore "ITC.Authorization/ITC.Authorization.csproj"
COPY . .
WORKDIR "/src/ITC.Authorization"
RUN dotnet build "./ITC.Authorization.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ITC.Authorization.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ITC.Authorization.dll"]
