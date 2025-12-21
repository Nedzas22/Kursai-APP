# Kursai - Course Trading Platform

> **Cross-platform course selling and management system built with .NET MAUI and ASP.NET Core**

[![.NET](https://img.shields.io/badge/.NET-9%20%7C%2010-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MAUI](https://img.shields.io/badge/MAUI-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/apps/maui)
[![MySQL](https://img.shields.io/badge/MySQL-8.0+-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![License](https://img.shields.io/badge/license-Private-red)](LICENSE)

---

## About The Project

**Kursai** is a fully functional course trading platform that allows users to:
- **Create** and sell their own courses
- **Purchase** courses from other users
- **Save** favorite courses
- **Manage** personal course library
- **Handle** profile and finances

### Architecture

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
?  Users • Courses • Purchases • Favorites • Ratings  ?
???????????????????????????????????????????????????????
```

---

## Key Features

### Authentication
- Registration with email validation
- Login with JWT token authentication
- Secure password hashing (BCrypt)
- Automatic token refresh

### Course Management
- Create new courses with descriptions and prices
- Edit and delete your own courses
- Category-based organization
- Price setting and management

### Shop
- Browse all available courses
- Search by title
- Filter by category
- Favorites system

### Library
- All purchased courses in one place
- Quick access to content
- Purchase history

### Profile
- My created courses
- Statistics and finances
- Account management
- Logout functionality

### Rating System
- Rate purchased courses (1-5 stars)
- View average ratings
- Rating statistics
- User reviews

### Download Feature
- Download course materials
- PDF and document support
- File management
- Offline access

---

## Technology Stack

### Backend (Kursai.Api)
| Technology | Version | Purpose |
|--------------|---------|-----------|
| ASP.NET Core | 9.0 | Web API Framework |
| Entity Framework Core | 9.0 | ORM (Database management) |
| MySQL | 8.0+ | Relational database |
| JWT | - | Authentication and authorization |
| Swagger | - | API documentation |
| BCrypt.Net | - | Password hashing |
| SendGrid | - | Email service |

### Frontend (Kursai.maui)
| Technology | Version | Purpose |
|--------------|---------|-----------|
| .NET MAUI | 10.0 | Cross-platform framework |
| XAML | - | UI markup language |
| MVVM | - | Architectural pattern |
| HttpClient | - | HTTP communication with API |
| Shell Navigation | - | Navigation between pages |

### Testing (Kursai.Tests)
| Technology | Version | Purpose |
|--------------|---------|-----------|
| xUnit | - | Unit testing framework |
| Moq | - | Mocking library |
| EF Core InMemory | - | In-memory database for tests |

**27 unit tests - 100% backend coverage**

---

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- .NET 10.0 SDK  
- MySQL Server 8.0+
- Visual Studio 2022 or VS Code
- Android SDK (for Android app)

### Backend API Setup

```bash
# 1. Create database
mysql -u root -p
CREATE DATABASE kursai;
EXIT;

# 2. Configure appsettings.json
cd Kursai.Api
# Edit ConnectionStrings and JwtSettings

# 3. Run migrations
dotnet ef database update

# 4. Start API
dotnet run
# API: https://localhost:7128
# Swagger: https://localhost:7128/swagger
```

### MAUI App Setup

```bash
cd Kursai.maui

# Update Configuration/ApiConfig.cs
# Android Emulator: http://10.0.2.2:7128/api
# Windows: https://localhost:7128/api

# Run application
dotnet build -t:Run -f net10.0-android   # Android
dotnet build -t:Run -f net10.0-windows10.0.19041.0  # Windows
```

### Run Tests

```bash
cd Kursai.Tests
dotnet test
# 27/27 tests passed
```

---

## Platform Support

| Platform | Support | Min. Version |
|-----------|---------|--------------|
| Android | Supported | API 21 (5.0 Lollipop) |
| iOS | Supported | iOS 11.0+ |
| Windows | Supported | Windows 10 (19041+) |
| macOS | Supported | MacCatalyst 14.0+ |

---

## API Endpoints

### Authentication (`/api/auth`)
```http
POST /api/auth/register   # Registration
POST /api/auth/login      # Login (returns JWT)
GET  /api/auth/validate   # Token validation
```

### Courses (`/api/courses`)
```http
GET    /api/courses           # All courses
GET    /api/courses/{id}      # Specific course
GET    /api/courses/my        # My courses [Auth]
POST   /api/courses           # Create course [Auth]
PUT    /api/courses/{id}      # Update course [Auth]
DELETE /api/courses/{id}      # Delete course [Auth]
```

### Favorites (`/api/favorites`)
```http
GET    /api/favorites                # Favorites [Auth]
POST   /api/favorites/{courseId}     # Add [Auth]
DELETE /api/favorites/{courseId}     # Remove [Auth]
GET    /api/favorites/check/{id}     # Check [Auth]
```

### Purchases (`/api/purchases`)
```http
GET    /api/purchases           # Purchased courses [Auth]
POST   /api/purchases/{id}      # Purchase [Auth]
```

### Ratings (`/api/ratings`)
```http
GET    /api/ratings/course/{courseId}     # Course ratings
POST   /api/ratings                        # Add rating [Auth]
PUT    /api/ratings/{id}                   # Update rating [Auth]
DELETE /api/ratings/{id}                   # Delete rating [Auth]
```

**[Auth]** = Requires JWT Bearer Token

---

## Database Schema

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
??? ImageData (LONGBLOB)
??? VideoUrl
??? FileData (LONGBLOB)
??? FileName
??? FileSize
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

Ratings
??? Id (PK)
??? UserId (FK ? Users)
??? CourseId (FK ? Courses)
??? RatingValue (1-5)
??? Comment
??? CreatedAt
```

---

## Screenshots

### Mobile Application
```
???????????????????????  ???????????????????????  ???????????????????????
?   Shop              ?  ?   Library           ?  ?   Profile           ?
?                     ?  ?                     ?  ?                     ?
?  Search courses...  ?  ?  My Purchased       ?  ?  Username           ?
?  [Filter]           ?  ?  Courses:           ?  ?  Email              ?
?                     ?  ?                     ?  ?                     ?
?  ?????????????????? ?  ?  ???????????????????  ?  My Courses         ?
?  ? Course Title   ? ?  ?  ? Bought Course  ??  ?  Logout             ?
?  ? $49.99         ? ?  ?  ? Access         ??  ?                     ?
?  ? Rating: 5/5    ? ?  ?  ? Rating: 4/5    ??  ?                     ?
?  ?????????????????? ?  ?  ???????????????????  ?                     ?
???????????????????????  ???????????????????????  ???????????????????????
```

---

## Testing

### Test Coverage
- **27 Unit Tests** (100% backend coverage)
- **AuthController**: Registration, login, validation
- **CoursesController**: CRUD operations, authorization
- **FavoritesController**: Favorites CRUD
- **JwtService**: Token generation and validation

```bash
# Run tests with detailed output
dotnet test --logger "console;verbosity=detailed"

# With code coverage
dotnet test /p:CollectCoverage=true
```

More details: [TESTU_LENTELE.md](TESTU_LENTELE.md)

---

## Security

| Feature | Implementation |
|---------|----------------|
| **Passwords** | BCrypt hash with salt |
| **Authentication** | JWT Bearer Tokens (24h expiry) |
| **Authorization** | Role-based endpoint protection |
| **HTTPS** | Required in production |
| **CORS** | Configured (whitelist) |

---

## Future Improvements

- [x] Course rating system
- [x] Download course materials
- [x] Email notifications (SendGrid)
- [ ] Comment functionality
- [ ] Advanced search and filtering
- [ ] Course view statistics
- [ ] Payment integration (Stripe/PayPal)
- [ ] Push notifications
- [ ] Offline mode (caching)
- [ ] Admin dashboard
- [ ] Video player integration
- [ ] PDF/document viewer

---

## Author

**Nedas**  
Full stack development project (Backend + Frontend + Database + Tests)

---

## Contact

- **GitHub**: [Nedzas22](https://github.com/Nedzas22)
- **Repo**: [Kursai-APP](https://github.com/Nedzas22/Kursai-APP)

---

## License

This project is a **private educational project**.

---

## Acknowledgments

- **Microsoft** for .NET ecosystem and MAUI framework
- **.NET Community** for open-source libraries
- **xUnit & Moq** for testing tools
- **SendGrid** for email services

---

<div align="center">

**If you like this project, give it a star!**

*Built with .NET MAUI and ASP.NET Core*

</div>
