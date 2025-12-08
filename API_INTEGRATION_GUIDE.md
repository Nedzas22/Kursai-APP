# API Integration Setup Guide

## ?? Problem Solved

Your app was using **in-memory storage** which caused:
- ? Data lost on app restart
- ? Courses not appearing across accounts
- ? No real data persistence

Now with **API integration**:
- ? Data persists in MySQL database
- ? Courses visible across all users
- ? Real-time data synchronization
- ? Secure authentication with JWT

## ?? Setup Steps

### 1. **Start the API Server**

```bash
cd C:\Users\Nedas\source\repos\Kursai\Kursai.Api
dotnet run
```

The API should start on: `https://localhost:7157` (check console for exact port)

### 2. **Update API URL in MAUI App**

Open `Kursai.maui/Configuration/ApiConfig.cs` and verify the URL matches your API port:

```csharp
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:7157/api";  // Android Emulator
#else
    public const string BaseUrl = "https://localhost:7157/api"; // Windows/iOS
#endif
```

**Important**: Change `7157` to match your API port if different!

### 3. **Run Both Projects**

#### Option A: Multiple Startup Projects (Recommended)
1. Right-click solution ? **"Configure Startup Projects"**
2. Select **"Multiple startup projects"**
3. Set both to **"Start"**:
   - `Kursai.Api` - Start
   - `Kursai.maui` - Start
4. Click **OK**
5. Press **F5** to run both

#### Option B: Run Separately
1. **Terminal 1**: `cd Kursai.Api && dotnet run`
2. **Visual Studio**: Run Kursai.maui (F5)

### 4. **Test the Integration**

1. **Login** with demo account:
   - Username: `demo`
   - Password: `demo123`

2. **Create a course**:
   - Go to Profile ? Create New Course
   - Fill in details
   - Submit

3. **Verify persistence**:
   - Course should appear in Shop immediately
   - Course should appear in "My Created Courses"
   - **Restart the app** - course should still be there!
   - **Login from different account** - course should be visible

## ?? What Changed

### New Files Created:
```
Kursai.maui/
??? Services/
?   ??? ApiAuthService.cs      (NEW - API-based authentication)
?   ??? ApiCourseService.cs    (NEW - API-based course management)
?   ??? AuthService.cs         (OLD - in-memory, backup)
?       CourseService.cs       (OLD - in-memory, backup)
??? Configuration/
    ??? ApiConfig.cs           (NEW - API URL configuration)
```

### Modified Files:
- `MauiProgram.cs` - Now uses API services instead of in-memory services

### Service Comparison:

| Feature | Old (In-Memory) | New (API) |
|---------|----------------|-----------|
| **Data Storage** | Memory only | MySQL Database |
| **Persistence** | Lost on restart | Permanent |
| **Multi-user** | ? Separate data | ? Shared data |
| **Authentication** | Fake | JWT tokens |
| **Real-time** | ? No | ? Yes |

## ?? Troubleshooting

### Problem: "Cannot connect to API"

**Solution**:
1. Verify API is running: `https://localhost:7157/swagger`
2. Check firewall settings
3. Update `ApiConfig.cs` with correct port
4. For Android Emulator, use `http://10.0.2.2:7157` (not localhost)

### Problem: "Unauthorized" errors

**Solution**:
1. Check you're logged in
2. Token might be expired - logout and login again
3. Verify JWT settings in `appsettings.json`

### Problem: Courses not appearing

**Solution**:
1. Check API is running
2. Check database connection in `appsettings.json`
3. Run migrations: `dotnet ef database update`
4. Check API logs in console

### Problem: Self-signed certificate error

**Solution**: Already handled! HttpClient is configured to accept self-signed certificates in development.

## ?? API Endpoints Being Used

### Authentication:
- `POST /api/auth/login` - Login user
- `POST /api/auth/register` - Register new user
- `GET /api/auth/validate` - Validate token

### Courses:
- `GET /api/courses` - Get all courses
- `GET /api/courses/{id}` - Get specific course
- `GET /api/courses/my-courses` - Get user's courses
- `POST /api/courses` - Create course
- `PUT /api/courses/{id}` - Update course
- `DELETE /api/courses/{id}` - Delete course

### Favorites:
- `GET /api/favorites` - Get favorites
- `POST /api/favorites/{courseId}` - Add favorite
- `DELETE /api/favorites/{courseId}` - Remove favorite
- `GET /api/favorites/check/{courseId}` - Check if favorited

### Purchases:
- `GET /api/purchases` - Get purchases
- `POST /api/purchases/{courseId}` - Purchase course

## ?? Benefits of API Integration

1. **Real Data Persistence**
   - Courses saved permanently in MySQL
   - Survive app restarts
   - Accessible from any device

2. **Multi-User Support**
   - All users see the same courses
   - Real marketplace functionality
   - User-specific favorites and purchases

3. **Security**
   - JWT token authentication
   - Secure password hashing (BCrypt)
   - Protected API endpoints

4. **Scalability**
   - Can support thousands of users
   - Can add web interface
   - Can deploy to cloud

5. **Professional Architecture**
   - Separation of concerns
   - API can serve multiple clients
   - Easy to maintain and update

## ? Success Checklist

- [ ] API server running on https://localhost:7157
- [ ] MySQL database running
- [ ] MAUI app builds successfully
- [ ] Can login with demo/demo123
- [ ] Can create new course
- [ ] Course appears in Shop immediately
- [ ] Course persists after app restart
- [ ] Course visible to other users

## ?? You're Done!

Your app now has:
- ? Real database persistence (MySQL)
- ? RESTful API backend
- ? JWT authentication
- ? Multi-user support
- ? Professional architecture

**Your courses will now persist across restarts and be visible to all users!** ??

## ?? Next Steps

1. Deploy API to Azure/AWS
2. Update `ApiConfig.cs` with production URL
3. Add offline mode support
4. Implement caching for better performance
5. Add image upload functionality
