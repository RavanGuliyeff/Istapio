FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Presentation/Istapio.API/Istapio.API.csproj", "Presentation/Istapio.API/"]
COPY ["src/Core/Istapio.Application/Istapio.Application.csproj", "src/Core/Istapio.Application/"]
COPY ["src/Core/Istapio.Domain/Istapio.Domain.csproj", "src/Core/Istapio.Domain/"]
COPY ["src/Infrastructure/Istapio.Infrastructure/Istapio.Infrastructure.csproj", "src/Infrastructure/Istapio.Infrastructure/"]
COPY ["src/Infrastructure/Istapio.Persistence/Istapio.Persistence.csproj", "src/Infrastructure/Istapio.Persistence/"]

RUN dotnet restore "./Presentation/Istapio.API/Istapio.API.csproj"
COPY . .
WORKDIR "/src/Presentation/Istapio.API"
RUN dotnet build "./Istapio.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Istapio.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Istapio.API.dll"]