# 🎮 GameShop

Aplikacja webowa do zarządzania sklepem z grami video zbudowana w ASP.NET Core 8.0 z Entity Framework Core i SQL Server.

---

## 📁 Struktura projektu

```
game-shop/
├── Controllers/              # Kontrolery MVC
│   ├── AccountController.cs      # Rejestracja, logowanie
│   ├── CategoriesController.cs   # Zarządzanie kategoriami (Admin)
│   ├── GamesController.cs        # Zarządzanie grami
│   ├── HomeController.cs         # Strona główna
│   ├── OrdersController.cs       # Zamówienia
│   └── PublishersController.cs   # Zarządzanie wydawcami (Admin)
│
├── Models/                   # Modele domenowe
│   ├── Category.cs              # Kategorie gier
│   ├── Game.cs                  # Gry
│   ├── Order.cs                 # Zamówienia
│   ├── OrderItem.cs             # Pozycje zamówienia
│   ├── Platform.cs              # Enum platform (PlayStation, Xbox, NintendoSwitch)
│   ├── Publisher.cs             # Wydawcy
│   ├── User.cs                  # Użytkownicy (Identity)
│   └── AccountViewModels.cs     # ViewModele dla Account
│
├── Data/                     # Warstwa danych
│   ├── GameShopContext.cs       # DbContext EF Core
│   └── SeedData.cs              # Inicjalizacja danych (role, admin)
│
├── Views/                    # Widoki Razor
│   ├── Shared/
│   │   └── _Layout.cshtml       # Główny layout (nawigacja, stopka)
│   ├── Home/
│   │   └── Index.cshtml         # Strona główna
│   ├── Games/                   # CRUD dla gier
│   ├── Categories/              # CRUD dla kategorii (Admin)
│   ├── Publishers/              # CRUD dla wydawców (Admin)
│   ├── Orders/                  # Zarządzanie zamówieniami
│   └── Account/                 # Login, Register
│
├── wwwroot/                  # Pliki statyczne
│   ├── css/
│   │   └── site.css             # Nowoczesny CSS (gradienty, animacje)
│   └── js/
│       └── site.js              # JavaScript (animacje, walidacja, filtry)
│
├── GameShop.Tests/           # Projekt testowy
│   ├── Models/                  # Testy jednostkowe dla modeli
│   └── Integration/             # Testy integracyjne dla kontrolerów
│       ├── CustomWebApplicationFactory.cs
│       └── Controllers/         # Testy kontrolerów
│
├── Program.cs                # Punkt wejścia aplikacji
├── GameShop.csproj           # Plik projektu
├── Dockerfile                # Docker dla aplikacji
├── Dockerfile.tests          # Docker dla testów
├── docker-compose.yml        # Orchestracja (app + SQL Server)
└── README.md                 # Ten plik
```

---

## 🛠 Technologie

### Backend
- **ASP.NET Core 8.0** - Framework webowy
- **Entity Framework Core 8.0** - ORM
- **SQL Server 2022** - Baza danych
- **ASP.NET Core Identity** - Autoryzacja i autentykacja

### Frontend
- **Razor Pages** - Silnik widoków
- **Bootstrap 5** - Framework CSS
- **Vanilla JavaScript** - Interakcje UI
- **CSS3** - Gradienty, animacje, responsywność

### Testy
- **xUnit 2.6.5** - Framework testowy
- **FluentAssertions 6.12.0** - Asercje
- **Microsoft.AspNetCore.Mvc.Testing 8.0.0** - Testy integracyjne
- **EntityFrameworkCore.InMemory 8.0.0** - Baza testowa
- **Moq 4.20.70** - Mockowanie

### DevOps
- **Docker** - Konteneryzacja
- **Docker Compose** - Orchestracja kontenerów

---

## 🗄 Baza danych

### Schemat bazy danych

```
AspNetUsers (Identity)          Categories               Publishers
├── Id (PK)                     ├── Id (PK)              ├── Id (PK)
├── Email                       ├── Name                 └── Name
├── FirstName                   └── Games (1:N)
├── LastName                              │
├── RegisteredAt                          │
└── Orders (1:N)                          ▼
          │                          Games
          │                     ├── Id (PK)
          ▼                     ├── Title
     Orders                     ├── Description
├── Id (PK)                     ├── Price
├── UserId (FK)                 ├── ReleaseDate
├── OrderDate                   ├── Stock
├── Status                      ├── CategoryId (FK)
├── TotalAmount                 ├── PublisherId (FK)
└── OrderItems (1:N)            ├── GamePlatform (Enum)
          │                     └── OrderItems (1:N)
          ▼
     OrderItems
├── Id (PK)
├── OrderId (FK)
├── GameId (FK)
├── Quantity
└── Price
```

### Tabele

#### **AspNetUsers** (ASP.NET Identity)
- Użytkownicy aplikacji
- Pola: Email, FirstName, LastName, RegisteredAt
- Role: Admin, User

#### **Categories**
- Kategorie gier (Action, RPG, Strategy, etc.)

#### **Publishers**
- Wydawcy gier

#### **Games**
- Katalog gier
- Relacje: Category (N:1), Publisher (N:1)
- Pola: Title, Description, Price, ReleaseDate, Stock, GamePlatform

#### **Orders**
- Zamówienia użytkowników
- Relacje: User (N:1)
- Pola: OrderDate, Status, TotalAmount

#### **OrderItems**
- Pozycje zamówienia (lista gier w zamówieniu)
- Relacje: Order (N:1), Game (N:1)
- Pola: Quantity, Price

### Migracje

Aplikacja używa `EnsureCreated()` zamiast migracji:
- Baza jest tworzona automatycznie przy pierwszym uruchomieniu
- **UWAGA:** `EnsureDeleted()` czyści bazę przy każdym restarcie (development only)

---

## 🚀 Uruchamianie aplikacji

### Metoda 1: Docker Compose (Zalecana) ✅

**Wymagania:** Docker Desktop

```powershell
# 1. Uruchom aplikację + SQL Server
docker-compose up --build

# 2. Otwórz w przeglądarce
http://localhost:5000

# 3. Zatrzymanie
docker-compose down
```

**Co się dzieje:**
- Uruchamia SQL Server 2022 na porcie 1433
- Buduje i uruchamia aplikację na porcie 5000
- Automatycznie tworzy bazę danych
- Inicjalizuje dane testowe (role, admin)

### Metoda 2: Lokalnie

**Wymagania:** .NET 8.0 SDK, SQL Server

```powershell
# 1. Ustaw connection string w appsettings.json
# (lub użyj zmiennej środowiskowej)

# 2. Uruchom aplikację
dotnet run

# 3. Otwórz adres z konsoli https://localhost:5000
```

### Metoda 3: Visual Studio

1. Otwórz `GameShop.sln`
2. Naciśnij `F5` lub kliknij ▶️
3. Aplikacja otworzy się w przeglądarce

---

## 🧪 Testy

### Struktura testów

```
GameShop.Tests/
├── Models/                    # 55 testów jednostkowych
│   ├── GameTests.cs              
│   ├── CategoryTests.cs          
│   ├── PublisherTests.cs         
│   ├── OrderTests.cs             
│   ├── OrderItemTests.cs         
│   ├── UserTests.cs              
│   └── PlatformTests.cs          
│
└── Integration/               # 55 testów integracyjnych
    ├── CustomWebApplicationFactory.cs  # Infrastruktura testowa
    └── Controllers/
        ├── HomeControllerTests.cs        
        ├── CategoriesControllerTests.cs  
        ├── PublishersControllerTests.cs  
        ├── GamesControllerTests.cs       
        ├── OrdersControllerTests.cs      
        └── AccountControllerTests.cs     
```

### Uruchamianie testów

#### Docker (Zalecane) ✅
```powershell
# Zbuduj obraz testowy
docker build -f Dockerfile.tests -t gameshop-tests .

# Uruchom wszystkie testy
docker run --rm gameshop-tests
```

#### Lokalnie
```powershell
# Wszystkie testy
dotnet test GameShop.Tests/GameShop.Tests.csproj

# Tylko testy jednostkowe
dotnet test --filter "FullyQualifiedName~Models"

# Tylko testy integracyjne
dotnet test --filter "FullyQualifiedName~Integration"

# Z szczegółami
dotnet test --verbosity detailed
```

### Wyniki testów

```
✅ Testy jednostkowe:  55/55 (100%)
✅ Testy integracyjne: 55/55 (100%)
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
✅ ŁĄCZNIE:           110/110 (100%)

Duration: ~1s
```

### Infrastruktura testowa

**Testy jednostkowe:**
- Testują modele domenowe w izolacji
- xUnit + FluentAssertions
- Szybkie wykonanie (<100ms)

**Testy integracyjne:**
- WebApplicationFactory (ASP.NET Core)
- InMemory Database (zamiast SQL Server)
- Testują pełny stack HTTP → Controller → DB
- Collection Fixture (współdzielona baza danych)
- Środowisko "Testing" (pomija EnsureDeleted)

---

## 👤 Dane testowe

Aplikacja automatycznie tworzy konta testowe przy pierwszym uruchomieniu:

### Konto Administratora
```
Email:    admin@gameshop.com
Hasło:    Admin123!
Rola:     Admin
```

**Dostęp do:**
- Wszystkich funkcji użytkownika
- Zarządzania kategoriami
- Zarządzania wydawcami
- Dodawania/edycji/usuwania gier

### Konto Użytkownika
```
Email:    user@gameshop.com
Hasło:    User123!
Rola:     User
```

**Dostęp do:**
- Przeglądania gier
- Składania zamówień
- Przeglądania własnych zamówień

---

## 🔧 Konfiguracja

### Connection String (docker-compose.yml)
```yaml
ConnectionStrings__DefaultConnection: "Server=sqlserver;Database=GameShopDB;User Id=sa;Password=StrongPassword123!;TrustServerCertificate=True;"
```

### SQL Server (docker-compose.yml)
```yaml
Environment:
  - SA_PASSWORD=StrongPassword123!
  - ACCEPT_EULA=Y
Port: 1433
```

### Aplikacja
```yaml
Port: 5000 (HTTP)
Depends on: sqlserver (healthy)
```

---

## 👨‍💻 Autorzy

Projekt stworzony jako aplikacja edukacyjna.  
Powstał w ramach projektu studenckiego przez:
- Bartosz Piróg
- Karol Mach
- Tymoteusz Łach


