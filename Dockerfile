# --- Étape 1 : Build de l'application ---
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copier le fichier projet depuis le sous-dossier
COPY ["EsportShop.Api/EsportShop.Api.csproj", "EsportShop.Api/"]
RUN dotnet restore "EsportShop.Api/EsportShop.Api.csproj"

# Copier tout le reste et compiler
COPY . .
WORKDIR "/src/EsportShop.Api"
RUN dotnet build "EsportShop.Api.csproj" -c Release -o /app/build

# --- Étape 2 : Publication ---
FROM build AS publish
RUN dotnet publish "EsportShop.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Étape 3 : Image finale pour l'exécution ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EsportShop.Api.dll"]