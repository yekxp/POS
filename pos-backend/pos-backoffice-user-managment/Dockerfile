FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["pos-backoffice-user-managment/pos-backoffice-user-managment.csproj", "pos-backoffice-user-managment/"]
RUN dotnet restore "./pos-backoffice-user-managment/pos-backoffice-user-managment.csproj"
COPY . .
WORKDIR "/src/pos-backoffice-user-managment"
RUN dotnet build "./pos-backoffice-user-managment.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./pos-backoffice-user-managment.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "pos-backoffice-user-managment.dll"]