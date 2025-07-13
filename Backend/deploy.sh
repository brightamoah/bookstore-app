#!/bin/bash

# Build and test the application
echo "Building and testing the application..."
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release --no-build

if [ $? -eq 0 ]; then
    echo "Build and tests successful!"
    
    # Build Docker image
    echo "Building Docker image..."
    docker build -t bookstore-api .
    
    if [ $? -eq 0 ]; then
        echo "Docker image built successfully!"
        echo "You can now deploy using:"
        echo "  - Railway: railway up"
        echo "  - Azure: az webapp create --deployment-container-image-name bookstore-api"
        echo "  - AWS: Push to ECR and deploy to ECS/Elastic Beanstalk"
    else
        echo "Docker build failed!"
        exit 1
    fi
else
    echo "Build or tests failed!"
    exit 1
fi
