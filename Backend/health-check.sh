#!/bin/bash

# Simple health check script for the deployed API
API_URL=${1:-"http://localhost:8080"}

echo "Testing BookStore API at: $API_URL"
echo "=================================="

# Test health endpoint
echo "1. Testing Health Endpoint..."
HEALTH_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/health")
if [ "$HEALTH_RESPONSE" -eq 200 ]; then
    echo "✅ Health check passed"
else
    echo "❌ Health check failed (HTTP $HEALTH_RESPONSE)"
fi

# Test API docs
echo "2. Testing API Documentation..."
SWAGGER_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/swagger/index.html")
if [ "$SWAGGER_RESPONSE" -eq 200 ]; then
    echo "✅ Swagger documentation accessible"
else
    echo "❌ Swagger documentation failed (HTTP $SWAGGER_RESPONSE)"
fi

# Test books endpoint (should return 401 without auth)
echo "3. Testing Books API..."
BOOKS_RESPONSE=$(curl -s -o /dev/null -w "%{http_code}" "$API_URL/api/books")
if [ "$BOOKS_RESPONSE" -eq 401 ]; then
    echo "✅ Books API responding (401 Unauthorized as expected)"
elif [ "$BOOKS_RESPONSE" -eq 200 ]; then
    echo "✅ Books API responding (200 OK)"
else
    echo "❌ Books API failed (HTTP $BOOKS_RESPONSE)"
fi

echo "=================================="
echo "Health check completed!"
