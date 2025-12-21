# Kursai - Kurs? Prekybos Platforma

## ?? Projekto Apžvalga

**Kursai** yra pilna kurs? prekybos platforma, sudaryta iš **REST API backend'o** ir **cross-platform mobiliosios aplikacijos**. Sistema leidžia vartotojams kurti, parduoti ir pirkti kursus, valdyti m?gstamiausius kursus ir tvarkyti asmenin? bibliotek?.

### ?? Pagrindin? Funkcionalumas

- ? Vartotoj? autentifikacija ir autorizacija (JWT)
- ? Kurs? k?rimas ir valdymas
- ? Kurs? parduotuv? (Marketplace)
- ? M?gstamiausi? kurs? sistema
- ? Asmenin? kurs? biblioteka
- ? Kurs? pirkimo funkcionalumas
- ? Vartotojo profilis

---

## ??? Architekt?ra

Projektas sudarytas iš **3 pagrindini? komponent?**:

```
Kursai/
??? Kursai.Api/          # ASP.NET Core Web API (Backend)
??? Kursai.maui/         # .NET MAUI aplikacija (Frontend)
??? Kursai.Tests/        # Unit testai (xUnit)
```

### 1. **Kursai.Api** - Backend API (.NET 9)

RESTful API serveris, kuris tvarko vis? verslo logik? ir duomen? valdym?.

#### ??? Technologijos:
- **Framework**: ASP.NET Core 9.0
- **Duomen? baz?**: MySQL su Entity Framework Core
- **Autentifikacija**: JWT (JSON Web Tokens)
- **API dokumentacija**: Swagger/OpenAPI
- **Saugumas**: BCrypt slaptažodži? hash'avimui

#### ?? Strukt?ra:
```
Kursai.Api/
??? Controllers/          # API endpoint'ai
?   ??? AuthController.cs        # Registracija ir prisijungimas
?   ??? CoursesController.cs     # Kurs? CRUD operacijos
?   ??? FavoritesController.cs   # M?gstamiausi? valdymas
?   ??? PurchasesController.cs   # Pirkim? valdymas
??? Models/              # Duomen? modeliai
?   ??? User.cs          # Vartotojo modelis
?   ??? Course.cs        # Kurso modelis
?   ??? Favorite.cs      # M?gstamiausio ryšys
?   ??? Purchase.cs      # Pirkimo ?rašas
??? DTOs/                # Data Transfer Objects
?   ??? AuthDtos.cs      # Login/Register DTO
?   ??? CourseDtos.cs    # Course CRUD DTO
??? Services/            # Verslo logika
?   ??? JwtService.cs    # JWT token? generavimas
??? Data/                # Duomen? baz?s kontekstas
?   ??? ApplicationDbContext.cs
??? Migrations/          # EF Core migracijos
```

#### ?? API Endpoint'ai:

**Authentication** (`/api/auth`)
- `POST /register` - Naujo vartotojo registracija
- `POST /login` - Prisijungimas (gr?žina JWT token)

**Courses** (`/api/courses`)
- `GET /` - Gauti visus kursus
- `GET /{id}` - Gauti konkret? kurs?
- `GET /my` - Gauti mano sukurtus kursus (reikia autentifikacijos)
- `POST /` - Sukurti nauj? kurs? (reikia autentifikacijos)
- `PUT /{id}` - Atnaujinti kurs? (tik savininkas)
- `DELETE /{id}` - Ištrinti kurs? (tik savininkas)

**Favorites** (`/api/favorites`)
- `GET /` - Gauti m?gstamiausius kursus (reikia autentifikacijos)
- `POST /{courseId}` - Prid?ti ? m?gstamiausius
- `DELETE /{courseId}` - Pašalinti iš m?gstamiausi?
- `GET /check/{courseId}` - Patikrinti ar kursas m?gstamiausias

**Purchases** (`/api/purchases`)
- `GET /` - Gauti pirktus kursus (reikia autentifikacijos)
- `POST /{courseId}` - Nusipirkti kurs?

---

### 2. **Kursai.maui** - Mobile App (.NET 10)

Cross-platform mobili? aplikacija, sukurta su .NET MAUI. Veikia **Android, iOS, Windows, MacCatalyst** platformose.

#### ??? Technologijos:
- **Framework**: .NET MAUI 10.0
- **Pattern**: MVVM (Model-View-ViewModel)
- **UI**: XAML
- **Navigation**: Shell Navigation
- **HTTP**: HttpClient su REST API

#### ?? Strukt?ra:
```
Kursai.maui/
??? Views/                    # XAML puslapiai
?   ??? LoginPage.xaml        # Prisijungimo ekranas
?   ??? RegisterPage.xaml     # Registracijos ekranas
?   ??? ShopPage.xaml         # Kurs? parduotuv?
?   ??? LibraryPage.xaml      # Asmenin? biblioteka
?   ??? ProfilePage.xaml      # Vartotojo profilis
?   ??? MyCoursesPage.xaml    # Mano sukurti kursai
?   ??? CourseDetailsPage.xaml # Kurso detal?s
?   ??? AddKursai.xaml        # Naujo kurso prid?jimas
?   ??? EditKursai.xaml       # Kurso redagavimas
??? ViewModels/               # MVVM View Models
?   ??? LoginViewModel.cs
?   ??? RegisterViewModel.cs
?   ??? ShopViewModel.cs
?   ??? LibraryViewModel.cs
?   ??? ProfileViewModel.cs
?   ??? MyCoursesViewModel.cs
?   ??? CourseDetailsViewModel.cs
?   ??? AddCourseViewModel.cs
?   ??? EditCourseViewModel.cs
?   ??? BaseViewModel.cs      # Bazin? ViewModel klas?
??? Models/                   # Duomen? modeliai
?   ??? User.cs
?   ??? Course.cs
?   ??? Purchase.cs
?   ??? Favorite.cs
??? Services/                 # API servisai
?   ??? IAuthService.cs
?   ??? ApiAuthService.cs     # Autentifikacijos servisas
?   ??? ICourseService.cs
?   ??? ApiCourseService.cs   # Kurs? servisas
??? Converters/               # XAML konverteriai
?   ??? InvertedBoolConverter.cs
?   ??? IsNotNullOrEmptyConverter.cs
??? Configuration/
?   ??? ApiConfig.cs          # API konfig?racija
??? AppShell.xaml             # Shell navigacija ir tab bar
```

#### ?? Aplikacijos Strukt?ra:

**Tab Navigation:**
1. **Shop** - Kurs? parduotuv? su paieška ir filtravim?
2. **Library** - Nusipirkti kursai (asmenin? biblioteka)
3. **Profile** - Vartotojo profilis ir mano kursai

**Funkcionalumas:**
- ?? Login/Register su JWT autentifikacija
- ?? Naršyti ir pirkti kursus
- ?? M?gstamiausi? kurs? sistema
- ?? Asmenin? pirkt? kurs? biblioteka
- ?? Kurti ir redaguoti savus kursus
- ??? Ištrinti savus kursus
- ?? Profilio valdymas ir atsijungimas

---

### 3. **Kursai.Tests** - Unit Tests (.NET 9)

Išsamus unit test? rinkinys, apimantis vis? backend funkcionalum?.

#### ??? Technologijos:
- **Framework**: xUnit
- **Mocking**: Moq
- **Database**: Entity Framework In-Memory

#### ? Test Coverage:

**27 testai iš viso:**

| Kategorija | Test? Kiekis | Aprašymas |
|------------|--------------|-----------|
| **AuthController** | 5 | Registracija, prisijungimas, validacija |
| **CoursesController** | 11 | CRUD operacijos, autorizacija |
| **FavoritesController** | 8 | M?gstamiausi kursai (CRUD + validacija) |
| **JwtService** | 3 | JWT token generavimas ir claims |

Daugiau informacijos: [Kursai.Tests/README.md](Kursai.Tests/README.md)

---

## ?? Kaip Paleisti Projekt?

### Reikalavimai

- **.NET 9.0 SDK** (API ir testams)
- **.NET 10.0 SDK** (MAUI aplikacijai)
- **MySQL Server** (duomen? bazei)
- **Visual Studio 2022** arba **Visual Studio Code**
- **Android SDK** (Android aplikacijai)

---

### 1. Backend API (Kursai.Api)

#### Konfig?racija

1. **Sukurti MySQL duomen? baz?:**
```sql
CREATE DATABASE KursaiDb;
```

2. **Redaguoti `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=KursaiDb;User=root;Password=YOUR_PASSWORD;"
  },
  "JwtSettings": {
    "SecretKey": "YOUR_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "Issuer": "KursaiAPI",
    "Audience": "KursaiApp",
    "ExpiryInHours": 24
  }
}
```

#### Paleisti API

```bash
cd Kursai.Api
dotnet restore
dotnet ef database update
dotnet run
```

API bus pasiekiamas: `https://localhost:7XXX` (portas nurodomas konsol?je)

**Swagger UI**: `https://localhost:7XXX/swagger`

---

### 2. MAUI Aplikacija (Kursai.maui)

#### Konfig?racija

1. **Atnaujinti API adres? `Configuration/ApiConfig.cs`:**
```csharp
public static string BaseUrl = "https://YOUR_IP_ADDRESS:PORT/api";
```

**Android Emulator:**
- Lokalus API: `https://10.0.2.2:PORT/api`

**Tikras ?renginys:**
- Naudoti kompiuterio IP adres? vietin?je tinkl?: `https://192.168.X.X:PORT/api`

#### Paleisti Aplikacij?

**Visual Studio 2022:**
1. Atidaryti `Kursai.sln`
2. Pasirinkti `Kursai.maui` kaip startup projekt?
3. Pasirinkti ?rengin? (Android Emulator, Windows Machine, etc.)
4. Spauti F5 arba Run

**CLI:**
```bash
cd Kursai.maui

# Android
dotnet build -t:Run -f net10.0-android

# Windows
dotnet build -t:Run -f net10.0-windows10.0.19041.0

# iOS (Mac'e)
dotnet build -t:Run -f net10.0-ios
```

---

### 3. Paleisti Testus (Kursai.Tests)

```bash
cd Kursai.Tests
dotnet restore
dotnet test

# Su detaliu output'u
dotnet test --logger "console;verbosity=detailed"
```

---

## ?? Duomen? Baz?s Schema

### User (Vartotojas)
```sql
- Id (INT, PK, Auto Increment)
- Username (VARCHAR(50), Unique)
- Email (VARCHAR(100), Unique)
- PasswordHash (TEXT)
- CreatedAt (DATETIME)
```

### Course (Kursas)
```sql
- Id (INT, PK, Auto Increment)
- Title (VARCHAR(200))
- Description (TEXT)
- Price (DECIMAL(10,2))
- Category (VARCHAR(100))
- SellerId (INT, FK -> User.Id)
- CreatedAt (DATETIME)
```

### Favorite (M?gstamiausias)
```sql
- Id (INT, PK, Auto Increment)
- UserId (INT, FK -> User.Id)
- CourseId (INT, FK -> Course.Id)
- UNIQUE(UserId, CourseId)
```

### Purchase (Pirkimas)
```sql
- Id (INT, PK, Auto Increment)
- UserId (INT, FK -> User.Id)
- CourseId (INT, FK -> Course.Id)
- PurchaseDate (DATETIME)
- Price (DECIMAL(10,2))
- UNIQUE(UserId, CourseId)
```

---

## ?? Saugumas

- **Slaptažodžiai**: Hash'uojami su **BCrypt**
- **API Autentifikacija**: **JWT Bearer Tokens**
- **Autorizacija**: Controller metodai apsaugoti `[Authorize]` atributu
- **CORS**: Sukonfig?ruotas leisti cross-origin requests (developmentui)
- **HTTPS**: Privalomas production'e

---

## ?? Test? Lentel?

Piln? test? aprašym? rasite: [TESTU_LENTELE.md](TESTU_LENTELE.md)

**Test? prioritetai:**
- ?? **Aukštas prioritetas**: 13 test? (kritin?s funkcijos)
- ?? **Žemas prioritetas**: 7 testai (papildomos funkcijos)

---

## ??? Technologij? Stack'as

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- MySQL 8.0+
- JWT Authentication
- Swagger/OpenAPI
- BCrypt.Net

### Frontend (MAUI)
- .NET MAUI 10.0
- XAML
- MVVM Pattern
- HttpClient
- Shell Navigation
- CommunityToolkit.Mvvm

### Testing
- xUnit
- Moq
- EF Core InMemory

---

## ?? Platform Support

| Platforma | Status | Min. Versija |
|-----------|--------|--------------|
| **Android** | ? Supported | API 21 (Lollipop 5.0) |
| **iOS** | ? Supported | iOS 11.0+ |
| **Windows** | ? Supported | Windows 10.0.19041.0+ |
| **macOS** | ? Supported | MacCatalyst 14.0+ |

---

## ?? API Response Format'ai

### S?kmingas Atsakymas
```json
{
  "id": 1,
  "title": "C# Programavimo Kursas",
  "description": "Išsamus C# kursas pradedantiesiems",
  "price": 49.99,
  "category": "Programavimas",
  "sellerId": 2,
  "createdAt": "2024-12-08T10:30:00Z"
}
```

### Klaidos Atsakymas
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "errors": {
    "Message": ["Course not found"]
  }
}
```

---

## ?? Development Workflow

1. **Backend Development**
   - Keisti modelius ? Sukurti migracij? ? Atnaujinti DB
   - Prid?ti endpoint'us ? Dokumentuoti Swagger'e
   - Rašyti unit testus

2. **Frontend Development**
   - Sukurti View ? Sukurti ViewModel ? Susieti
   - Implementuoti API servisus
   - Testuoti ?vairiose platformose

3. **Testing**
   - Rašyti unit testus kiekvienam feature'ui
   - Testuoti API endpoint'us Swagger UI
   - Testuoti MAUI app'? Android/Windows

---

## ?? Known Issues / Future Improvements

### Planuojami Patobulinimai:
- [ ] Kurs? reiting? sistema
- [ ] Komentar? funkcionalumas
- [ ] Kurs? kategorij? filtravimas
- [ ] Paieškos tobulinimas
- [ ] Push notifications
- [ ] Offline režimas (caching)
- [ ] Mok?jim? integracijos (Stripe, PayPal)
- [ ] Admin dashboard
- [ ] Kurs? perži?ros statistika

---

## ?? Autoriai

- **Vardas Pavard?** - Pilnas projekto k?rimas

---

## ?? Licencija

Šis projektas yra privatus mokomasis projektas.

---

## ?? Kontaktai

Jei turite klausim? ar pasi?lym?:
- GitHub: https://github.com/Nedzas22/Kursai-APP
- Email: [Your Email]

---

## ?? Pad?kos

- Microsoft už .NET MAUI framework'?
- .NET Community už puikias bibliotekas
- xUnit ir Moq už testing tools

---

**Projektas sukurtas 2024 m. kaip mokomasis projektas.**
