# Login/Register Issue - Fixed! ?

## ?? Problem
- Login showing "Invalid username or password"
- Register showing "Username or email already used"
- App couldn't connect to API

## ?? Root Cause
**Port Mismatch!** The MAUI app was configured to connect to port `7157`, but the API was actually running on port `7128`.

## ? What Was Fixed

### 1. **Updated API Configuration**
- Changed port from `7157` ? `7128` in `ApiConfig.cs`
- Windows/iOS: `https://localhost:7128/api`
- Android: `http://10.0.2.2:7128/api`

### 2. **Added Better Error Messages**
- Now shows "Cannot connect to API server" if API is not running
- Shows actual error details in debug output
- More helpful error messages for users

### 3. **Improved Debugging**
- Added console logging to see what's happening
- Better exception handling
- Clear error messages

## ?? How to Test

### Step 1: Verify API is Running
Open browser and go to: **https://localhost:7128/swagger**

You should see the Swagger API documentation.

### Step 2: Test Login
1. **Stop your MAUI app** (if running)
2. **Rebuild the solution** (Ctrl+Shift+B)
3. **Run the MAUI app**
4. Try logging in with:
   - Username: `demo`
   - Password: `demo123`

### Step 3: Test Registration
1. Click "Sign Up"
2. Enter:
   - Username: `testuser` (or any unique username)
   - Email: `test@example.com`
   - Password: `password123`
3. Should successfully register and login

## ?? Troubleshooting

### Problem: Still can't connect

**Check API Port:**
1. Look at the API console when it starts
2. Find the line: `Now listening on: https://localhost:XXXX`
3. Update `ApiConfig.cs` with the correct port

**Alternative:** Use HTTP instead of HTTPS:
```csharp
public const string BaseUrl = "http://localhost:5265/api";
```

### Problem: "Invalid username or password"

**Solution 1 - Use Demo Account:**
- Username: `demo`
- Password: `demo123`

**Solution 2 - Check Database:**
```sql
-- Connect to MySQL and check if users exist
USE kursai;
SELECT * FROM Users;
```

**Solution 3 - Recreate Database:**
```bash
cd Kursai.Api
dotnet ef database drop -f
dotnet ef database update
```

### Problem: "Username or email already exists"

**This is normal!** It means:
- That username is taken (try a different one)
- OR the account already exists (try logging in instead)

Try usernames like:
- `testuser2`
- `myuser`
- `student1`

### Problem: Certificate/SSL Error

**Solution:** Already handled! The app accepts self-signed certificates in development.

If still having issues, use HTTP:
```csharp
#if ANDROID
    public const string BaseUrl = "http://10.0.2.2:5265/api";
#else
    public const string BaseUrl = "http://localhost:5265/api";
#endif
```

## ?? How to Check What's Happening

### View Debug Output
In Visual Studio:
1. View ? Output
2. Select "Debug" from dropdown
3. Look for lines like:
   - `Attempting login to: https://localhost:7128/api/auth/login`
   - `Login response status: 200`
   - Any error messages

### Check API Logs
Look at the API console window for:
- Incoming HTTP requests
- Status codes (200 = success, 401 = unauthorized, 400 = bad request)
- Any error messages

### Test API Directly
Use Swagger UI at: `https://localhost:7128/swagger`

1. Click on `/api/auth/login`
2. Click "Try it out"
3. Enter credentials:
   ```json
   {
     "username": "demo",
     "password": "demo123"
   }
   ```
4. Click "Execute"
5. Should get 200 response with token

## ? Success Checklist

- [ ] API running on https://localhost:7128
- [ ] Swagger accessible at https://localhost:7128/swagger
- [ ] MAUI app rebuilt after changes
- [ ] Can login with demo/demo123
- [ ] Can register new users
- [ ] Error messages are helpful

## ?? Expected Behavior Now

### Login:
- ? Successful login ? Navigate to main app
- ? Wrong credentials ? Show "Invalid username or password"
- ? API not running ? Show "Cannot connect to API server"

### Register:
- ? Successful registration ? Auto-login and navigate to main app
- ? Username exists ? Show "Username or email already exists"
- ? Password too short ? Show "Password must be at least 6 characters"
- ? Passwords don't match ? Show "Passwords do not match"

## ?? Port Configuration Summary

| Component | Port | URL |
|-----------|------|-----|
| API (HTTPS) | 7128 | https://localhost:7128 |
| API (HTTP) | 5265 | http://localhost:5265 |
| Swagger | 7128 | https://localhost:7128/swagger |
| MAUI App | - | Connects to API port |

**Your app should now work!** ??

If you're still having issues, check the Debug output window in Visual Studio for detailed error messages.
