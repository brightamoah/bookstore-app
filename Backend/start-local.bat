@echo off

echo Starting BookStore API locally for testing...
echo =============================================

echo Setting environment variables...
set ASPNETCORE_ENVIRONMENT=Development
set ASPNETCORE_URLS=http://localhost:8080

echo Building the application...
dotnet build --configuration Release

if %errorlevel% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo Starting the API server...
echo API will be available at: http://localhost:8080
echo Swagger documentation: http://localhost:8080/swagger
echo Health check: http://localhost:8080/health
echo.
echo Press Ctrl+C to stop the server

dotnet run --configuration Release --urls "http://localhost:8080"
