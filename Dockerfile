# Użycie wersji .NET SDK 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Kopiowanie pliku projektu (.csproj) i przywracanie zależności
COPY ["GameShop.csproj", "./"]
RUN dotnet restore "./GameShop.csproj"

# Kopiowanie zawartości projektu do obrazu
COPY . .

# Kompilacja projektu
RUN dotnet build "./GameShop.csproj" -c Release -o /app/build

# Publikacja zoptymalizowanego kodu
RUN dotnet publish "./GameShop.csproj" -c Release -o /app/publish

# Utworzenie finalnego obrazu
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "GameShop.dll"]