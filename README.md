# ?? Kursai - Kurs? Prekybos Platforma

> **Cross-platform kurs? pardavimo ir valdymo sistema, sukurta su .NET MAUI ir ASP.NET Core**

[![.NET](https://img.shields.io/badge/.NET-9%20%7C%2010-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![MySQL](https://img.shields.io/badge/MySQL-8.0+-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/license-Private-red)](LICENSE)

---

## ?? Apie Projekt?

**Kursai** yra pilnai funkcionali kurs? prekybos platforma, leidžianti vartotojams:
- ?? **Kurti** ir parduoti savo kursus
- ?? **Pirkti** kursus iš kit? vartotoj?
- ?? **Išsaugoti** m?gstamus kursus
- ?? **Valdyti** asmenin? kurs? bibliotek?
- ?? **Tvarkyti** savo profil? ir finansus

### ??? Architekt?ra

```
???????????????????????????????????????????????????????
?           .NET MAUI Mobile App (Frontend)           ?
?  Android • iOS • Windows • macOS                    ?
???????????????????????????????????????????????????????
                     ? REST API (HTTPS/JWT)
                     ?
???????????????????????????????????????????????????????
?        ASP.NET Core Web API (Backend)               ?
?  JWT Auth • Swagger • Entity Framework Core         ?
???????????????????????????????????????????????????????
                     ?
???????????????????????????????????????????????????????
?              MySQL Database                         ?
?  Users • Courses • Purchases • Favorites            ?
???????????????????????????????????????????????????????
```

---

## ? Pagrindin?s Funkcijos

### ?? Autentifikacija
- Registracija su email validacija
- Prisijungimas su JWT token autentifikacija
- Saugus slaptažodži? hash'avimas (BCrypt)
- Automatinis token atnaujinimas

### ?? Kurs? Valdymas
- Kurti naujus kursus su aprašymais ir kainomis
- Redaguoti ir ištrinti savo kursus
- Kategorijomis pagr?sta organizacija
- Kain? nustatymas

### ??? Parduotuv? (Shop)
- Naršyti visus prieinamus kursus
- Paieška pagal pavadinim?
- Filtravimas pagal kategorij?
- M?gstamiausi? sistema

### ?? Biblioteka (Library)
- Visos nusipirktos kursai vienoje vietoje
- Greita prieiga prie turinio
- Pirkimo istorija

### ?? Profilis
- Mano sukurti kursai
- Statistika ir finansai
- Paskyros valdymas
- Atsijungimas

---

## ??? Technologij? Stack'as

### Backend (Kursai.Api)
| Technologija | Versija | Paskirtis |
|--------------|---------|-----------|
| ASP.NET Core | 9.0 | Web API Framework |
| Entity Framework Core | 9.0 | ORM (Duomen? baz?s valdymas) |
| MySQL | 8.0+ | Reliacin? duomen? baz? |
| JWT | - | Autentifikacija ir autorizacija |
| Swagger | - | API dokumentacija |
| BCrypt.Net | - | Slaptažodži? hash'avimas |

### Frontend (Kursai.maui)
| Technologija | Versija | Paskirtis |
|--------------|---------|-----------|
| .NET MAUI | 10.0 | Cross-platform framework |
| XAML | - | UI markup kalba |
| MVVM | - | Architekt?rinis pattern'as |
| HttpClient | - | HTTP komunikacija su API |
| Shell Navigation | - | Navigacija tarp puslapi? |

### Testing (Kursai.Tests)
| Technologija | Versija | Paskirtis |
|--------------|---------|-----------|
| xUnit | - | Unit testing framework |
| Moq | - | Mocking library |
| EF Core InMemory | - | In-memory database testams |

**27 unit testai** - 100% backend coverage! ?

---

## ?? Greitai Prad?ti

### Reikalavimai

```
? .NET 9.0 SDK
? .NET 10.0 SDK  
? MySQL Server 8.0+
? Visual Studio 2022 arba VS Code
? Android SDK (Android aplikacijai)
```

### 1?? Backend API Setup

```bash
# 1. Sukurti duomen? baz?
mysql -u root -p
CREATE DATABASE kursai;
EXIT;

# 2. Konfig?ruoti appsettings.json
cd Kursai.Api
# Redaguoti ConnectionStrings ir JwtSettings

# 3. Paleisti migracijas
dotnet ef database update

# 4. Paleisti API
dotnet run
# API: https://localhost:7128
# Swagger: https://localhost:7128/swagger
```

### 2?? MAUI App Setup

```bash
cd Kursai.maui

# Atnaujinti Configuration/ApiConfig.cs
# Android Emulator: http://10.0.2.2:7128/api
# Windows: https://localhost:7128/api

# Paleisti aplikacij?
dotnet build -t:Run -f net10.0-android   # Android
dotnet build -t:Run -f net10.0-windows10.0.19041.0  # Windows
```

### 3?? Paleisti Testus

```bash
cd Kursai.Tests
dotnet test
# ? 27/27 testai passed
```

---

## ?? Platform Support

| Platforma | Support | Min. Versija |
|-----------|---------|--------------|
| ?? Android | ? | API 21 (5.0 Lollipop) |
| ?? iOS | ? | iOS 11.0+ |
| ?? Windows | ? | Windows 10 (19041+) |
| ?? macOS | ? | MacCatalyst 14.0+ |

---

## ?? API Endpoints

### ?? Authentication (`/api/auth`)
```http
POST /api/auth/register   # Registracija
POST /api/auth/login      # Prisijungimas (returns JWT)
GET  /api/auth/validate   # Token validacija
```

### ?? Courses (`/api/courses`)
```http
GET    /api/courses           # Visi kursai
GET    /api/courses/{id}      # Konkretus kursas
GET    /api/courses/my        # Mano kursai [Auth]
POST   /api/courses           # Sukurti kurs? [Auth]
PUT    /api/courses/{id}      # Atnaujinti kurs? [Auth]
DELETE /api/courses/{id}      # Ištrinti kurs? [Auth]
```

### ?? Favorites (`/api/favorites`)
```http
GET    /api/favorites                # M?gstamiausi [Auth]
POST   /api/favorites/{courseId}     # Prid?ti [Auth]
DELETE /api/favorites/{courseId}     # Pašalinti [Auth]
GET    /api/favorites/check/{id}     # Patikrinti [Auth]
```

### ?? Purchases (`/api/purchases`)
```http
GET    /api/purchases           # Pirkti kursai [Auth]
POST   /api/purchases/{id}      # Nusipirkti [Auth]
```

**[Auth]** = Reikia JWT Bearer Token

---

## ??? Duomen? Baz?s Schema

```sql
Users
??? Id (PK)
??? Username (UNIQUE)
??? Email (UNIQUE)
??? PasswordHash
??? CreatedAt

Courses
??? Id (PK)
??? Title
??? Description
??? Price
??? Category
??? SellerId (FK ? Users)
??? CreatedAt

Favorites
??? Id (PK)
??? UserId (FK ? Users)
??? CourseId (FK ? Courses)

Purchases
??? Id (PK)
??? UserId (FK ? Users)
??? CourseId (FK ? Courses)
??? PurchaseDate
??? Price
```

---

## ?? Screenshots

### Mobil? Aplikacija
```
???????????????????????  ???????????????????????  ???????????????????????
?   ?? Shop           ?  ?   ?? Library        ?  ?   ?? Profile        ?
?                     ?  ?                     ?  ?                     ?
?  Search courses...  ?  ?  My Purchased       ?  ?  Username           ?
?  [Filter ?]         ?  ?  Courses:           ?  ?  Email              ?
?                     ?  ?                     ?  ?                     ?
?  ?????????????????? ?  ?  ???????????????????  ?  My Courses         ?
?  ? Course Title   ? ?  ?  ? Bought Course  ??  ?  Logout             ?
?  ? $49.99      ?? ? ?  ?  ? Access ?       ??  ?                     ?
?  ?????????????????? ?  ?  ???????????????????  ?                     ?
???????????????????????  ???????????????????????  ???????????????????????
```

---

## ?? Testing

### Test Coverage
- **? 27 Unit Test?** (100% backend coverage)
- **AuthController**: Registracija, login, validacija
- **CoursesController**: CRUD operacijos, autorizacija
- **FavoritesController**: M?gstamiausi CRUD
- **JwtService**: Token generavimas ir validacija

```bash
# Paleisti testus su detaliu output'u
dotnet test --logger "console;verbosity=detailed"

# Su code coverage
dotnet test /p:CollectCoverage=true
```

Daugiau: [?? TESTU_LENTELE.md](TESTU_LENTELE.md)

---

## ?? Saugumas

| Feature | Implementation |
|---------|----------------|
| ?? **Slaptažodžiai** | BCrypt hash su salt |
| ?? **Autentifikacija** | JWT Bearer Tokens (24h expiry) |
| ??? **Autorizacija** | Role-based endpoint protection |
| ?? **HTTPS** | Privalomas production'e |
| ?? **CORS** | Sukonfig?ruotas (whitelist) |

---

## ?? Dokumentacija

- [?? SYSTEM_ARCHITECTURE_EXPLAINED.md](SYSTEM_ARCHITECTURE_EXPLAINED.md) - Pilnas sistemos architekt?ros aprašymas
- [?? TESTU_LENTELE.md](TESTU_LENTELE.md) - Išsami test? lentel?
- [?? PROGRAMINES_IRANGOS_REALIZACIJA.md](PROGRAMINES_IRANGOS_REALIZACIJA.md) - Realizacijos dokumentas
- [?? Kursai.Tests/README.md](Kursai.Tests/README.md) - Unit test? dokumentacija

---

## ?? B?simi Patobulinimai

- [ ] ? Kurs? reiting? sistema
- [ ] ?? Komentar? funkcionalumas
- [ ] ?? Išpl?stin? paieška ir filtravimas
- [ ] ?? Kurs? perži?ros statistika
- [ ] ?? Mok?jim? integracija (Stripe/PayPal)
- [ ] ?? Push notifications
- [ ] ?? Offline režimas (caching)
- [ ] ????? Admin dashboard
- [ ] ?? Video player integracija
- [ ] ?? PDF/dokument? perži?ra

---

## ????? Autorius

**Nedas**  
Pilnas stack development projektas (Backend + Frontend + Database + Tests)

---

## ?? Kontaktai

- ?? **GitHub**: [Nedzas22](https://github.com/Nedzas22)
- ?? **Email**: laizytuvas1@gmail.com
- ?? **Repo**: [Kursai-APP](https://github.com/Nedzas22/Kursai-APP)

---

## ?? Licencija

Šis projektas yra **privatus mokomasis projektas**.

---

## ?? Pad?kos

- **Microsoft** už .NET ekosistem? ir MAUI framework'?
- **.NET Community** už open-source bibliotekas
- **xUnit & Moq** už testing tools

---

<div align="center">

**? Jei patiko projektas, palikite žvaigždut?! ?**

*Sukurta su ?? naudojant .NET MAUI ir ASP.NET Core*

</div>
