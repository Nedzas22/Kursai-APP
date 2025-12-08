# Kursai API - Complete Implementation Guide

## ?? What Has Been Implemented

### 1. **Entity Framework Core with MySQL**
- Full database context with relationships
- Entity models: User, Course, Favorite, Purchase
- Database migrations support
- Seed data for demo user and courses

### 2. **RESTful API Endpoints**

#### **Authentication Endpoints** (`/api/auth`)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
- `GET /api/auth/validate` - Validate existing token

#### **Courses Endpoints** (`/api/courses`)
- `GET /api/courses` - Get all courses (public)
- `GET /api/courses/{id}` - Get course by ID (public)
- `GET /api/courses/my-courses` - Get user's created courses (authenticated)
- `POST /api/courses` - Create new course (authenticated)
- `PUT /api/courses/{id}` - Update course (authenticated, owner only)
- `DELETE /api/courses/{id}` - Delete course (authenticated, owner only)

#### **Favorites Endpoints** (`/api/favorites`)
- `GET /api/favorites` - Get user's favorite courses
- `POST /api/favorites/{courseId}` - Add course to favorites
- `DELETE /api/favorites/{courseId}` - Remove course from favorites
- `GET /api/favorites/check/{courseId}` - Check if course is favorited

#### **Purchases Endpoints** (`/api/purchases`)
- `GET /api/purchases` - Get user's purchased courses
- `POST /api/purchases/{courseId}` - Purchase a course

### 3. **JWT Token Authentication**
- Secure password hashing with BCrypt
- JWT token generation and validation
- Token-based authentication for protected endpoints
- Claims-based authorization

## ?? Prerequisites

1. **.NET 10 SDK** installed
2. **MySQL Server** installed and running
3. **Visual Studio 2022** or **VS Code**
4. **MySQL Workbench** (optional, for database management)

## ?? Setup Instructions

### Step 1: Install Required Packages

```bash
cd C:\Users\Nedas\source\repos\Kursai\Kursai.Api

dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.0
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.0
dotnet add package Microsoft.IdentityModel.Tokens --version 8.2.1
dotnet add package System.IdentityModel.Tokens.Jwt --version 8.2.1
dotnet add package BCrypt.Net-Next --version 4.0.3
```

### Step 2: Configure MySQL Connection

1. Open `Kursai.Api/appsettings.json`
2. Update the connection string with your MySQL credentials:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=KursaiDb;User=root;Password=YOUR_MYSQL_PASSWORD;Port=3306"
  }
}
```

### Step 3: Create MySQL Database

Open MySQL Workbench or command line and run:

```sql
CREATE DATABASE KursaiDb;
```

### Step 4: Run Database Migrations

```bash
cd C:\Users\Nedas\source\repos\Kursai\Kursai.Api

dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Step 5: Run the API

```bash
dotnet run
```

The API will start at: `https://localhost:7xxx` (check console for exact port)

### Step 6: Test API with Swagger

1. Open browser and navigate to: `https://localhost:7xxx/swagger`
2. You'll see the interactive API documentation
3. Test the endpoints:
   - Register a new user
   - Login to get JWT token
   - Click "Authorize" button and enter: `Bearer YOUR_TOKEN`
   - Test protected endpoints

## ?? Authentication Flow

### 1. Register New User

```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "password123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 2,
  "username": "testuser",
  "email": "test@example.com"
}
```

### 2. Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "demo",
  "password": "demo123"
}
```

### 3. Use Token for Protected Endpoints

```http
GET /api/courses/my-courses
Authorization: Bearer YOUR_JWT_TOKEN
```

## ?? Database Schema

### Users Table
- Id (PK)
- Username (Unique)
- Email (Unique)
- PasswordHash
- CreatedAt

### Courses Table
- Id (PK)
- Title
- Description
- Price
- SellerId (FK ? Users)
- Category
- ImageUrl
- CreatedAt

### Favorites Table
- Id (PK)
- UserId (FK ? Users)
- CourseId (FK ? Courses)
- AddedDate
- Unique constraint on (UserId, CourseId)

### Purchases Table
- Id (PK)
- UserId (FK ? Users)
- CourseId (FK ? Courses)
- PurchaseDate
- Price
- Unique constraint on (UserId, CourseId)

## ?? Testing with Postman

### Collection Structure

1. **Auth**
   - Register
   - Login
   - Validate Token

2. **Courses**
   - Get All Courses
   - Get Course by ID
   - Get My Courses
   - Create Course
   - Update Course
   - Delete Course

3. **Favorites**
   - Get Favorites
   - Add to Favorites
   - Remove from Favorites
   - Check Favorite Status

4. **Purchases**
   - Get Purchases
   - Purchase Course

### Environment Variables (Postman)
```
baseUrl = https://localhost:7xxx
token = (set after login)
```

## ?? Troubleshooting

### Problem: "Unable to connect to MySQL server"
**Solution**: 
- Ensure MySQL server is running
- Check connection string in appsettings.json
- Verify MySQL user credentials

### Problem: "JWT Secret Key not configured"
**Solution**: 
- Ensure JwtSettings section exists in appsettings.json
- SecretKey must be at least 32 characters long

### Problem: Migration fails
**Solution**:
```bash
dotnet ef database drop -f
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## ?? Integrating with MAUI App

Update your MAUI app's services to call the API:

```csharp
public class ApiAuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7xxx/api";
    
    public async Task<User?> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{BaseUrl}/auth/login",
            new { username, password });
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            // Store token in secure storage
            await SecureStorage.SetAsync("auth_token", result.Token);
            return new User { 
                Id = result.UserId,
                Username = result.Username, 
                Email = result.Email 
            };
        }
        return null;
    }
}
```

## ?? Demo Credentials

**Username**: `demo`
**Password**: `demo123`

## ?? Additional Resources

- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [JWT.IO](https://jwt.io/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [ASP.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/)

## ? Summary

You now have a complete RESTful API with:
- ? Entity Framework Core with MySQL database
- ? JWT token-based authentication
- ? Secure password hashing with BCrypt
- ? RESTful endpoints for all CRUD operations
- ? Swagger documentation
- ? CORS enabled for MAUI app integration
- ? Relationship management (Users, Courses, Favorites, Purchases)
- ? Authorization for protected resources

**Total API Lines of Code**: ~1,500 lines
**Total Project Lines of Code**: ~4,500 lines (including API)

?? **Ready to deploy and integrate with your MAUI app!**
