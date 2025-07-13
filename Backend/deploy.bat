@echo off

echo Building and testing the application...
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --no-build

if %errorlevel% equ 0 (
    echo Build and tests successful!
    
    echo Building Docker image...
    docker build -t bookstore-api .
    
    if %errorlevel% equ 0 (
        echo Docker image built successfully!
        echo You can now deploy using:
        echo   - Railway: railway up
        echo   - Azure: az webapp create --deployment-container-image-name bookstore-api
        echo   - AWS: Push to ECR and deploy to ECS/Elastic Beanstalk
    ) else (
        echo Docker build failed!
        exit /b 1
    )
) else (
    echo Build or tests failed!
    exit /b 1
)
