# Kursai API - Package Installation Script
# Run this script from PowerShell to install all required NuGet packages

Write-Host "Installing NuGet packages for Kursai.Api..." -ForegroundColor Green

$projectPath = "C:\Users\Nedas\source\repos\Kursai\Kursai.Api\Kursai.Api.csproj"

# Install packages
dotnet add $projectPath package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add $projectPath package Microsoft.EntityFrameworkCore.Design --version 10.0.0
dotnet add $projectPath package Microsoft.AspNetCore.Authentication.JwtBearer --version 10.0.0
dotnet add $projectPath package Microsoft.IdentityModel.Tokens --version 8.2.1
dotnet add $projectPath package System.IdentityModel.Tokens.Jwt --version 8.2.1
dotnet add $projectPath package BCrypt.Net-Next --version 4.0.3

Write-Host ""
Write-Host "All packages installed successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Update MySQL connection string in appsettings.json"
Write-Host "2. Create database: CREATE DATABASE KursaiDb;"
Write-Host "3. Run migrations: dotnet ef migrations add InitialCreate"
Write-Host "4. Update database: dotnet ef database update"
Write-Host "5. Run API: dotnet run"
Write-Host ""
