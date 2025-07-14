# 📱 BookStore Backend API

A comprehensive ASP.NET Core 8.0 Web API for managing a bookstore with
authentication, file uploads, and rate limiting.

## 🚀 Quick Start

### Local Development

```bash
# Clone and navigate to backend
cd Backend

# Install dependencies
dotnet restore

# Run locally
dotnet run
```

The API will be available at:

-  **API**: http://localhost:8080
-  **Swagger**: http://localhost:8080/swagger
-  **Health**: http://localhost:8080/health

## 🔧 Features

-  ✅ **Authentication**: JWT-based with cookies
-  ✅ **Authorization**: Role-based access control
-  ✅ **File Upload**: Image handling with validation
-  ✅ **Rate Limiting**: Built-in request throttling
-  ✅ **Validation**: Comprehensive input validation
-  ✅ **CORS**: Configured for frontend integration
-  ✅ **Health Checks**: Monitoring endpoints
-  ✅ **Swagger**: API documentation
-  ✅ **Error Handling**: Global exception middleware

## 📁 Project Structure

```
Backend/
├── Controllers/          # API endpoints
├── Data/                # Repository pattern
├── Models/              # Domain models
├── DTOs/                # Data transfer objects
├── Middleware/          # Custom middleware
├── Helpers/             # Utility services
├── Migrations/          # EF Core migrations
└── wwwroot/uploads/     # File storage
```

## 🧪 Testing

```bash
# Run all tests
dotnet test

```

## 🔒 Security Features

-  Password hashing with BCrypt
-  JWT token authentication
-  Rate limiting protection
-  Input validation
-  CORS configuration
-  Global exception handling

## 📊 API Endpoints

### Authentication

-  `POST /api/signup` - User registration
-  `POST /api/login` - User login
-  `POST /api/logout` - User logout

### Books

-  `GET /api/books` - List all books
-  `GET /api/books/{id}` - Get book by ID
-  `POST /api/books` - Create new book
-  `PUT /api/books/{id}` - Update book
-  `DELETE /api/books/{id}` - Delete book
-  `GET /api/books/search` - Search books

### Health & Monitoring

-  `GET /health` - Health check
-  `GET /swagger` - API documentation

## 🚨 Troubleshooting

### Common Issues:

1. **Port already in use**

   ```bash
   netstat -tulpn | grep :8080
   kill -9 <process-id>
   ```

2. **Database connection failed**

   -  Check connection string format
   -  Verify database exists
   -  Check network connectivity

3. **CORS errors**

   -  Update frontend URL in `appsettings.json`
   -  Check CORS policy in `Startup.cs`

4. **File upload issues**
   -  Ensure `wwwroot/uploads` directory exists
   -  Check file permissions
   -  Verify file size limits
