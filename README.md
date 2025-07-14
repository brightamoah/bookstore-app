# BookStoreApp - Local Development Guide

This guide provides instructions for setting up and running the BookStoreApp
locally on your development machine.

## Prerequisites

-  **Backend**: .NET 8.0 SDK
   ([download](https://dotnet.microsoft.com/download/dotnet/8.0))
-  **Frontend**: Node.js 18.x or higher ([download](https://nodejs.org/))
-  **Database**: MySQL Server
   ([download](https://dev.mysql.com/downloads/mysql/))

## Database Setup

1. **Start MySQL Server** - Make sure MySQL is installed and running on your
   machine
2. **Create the database**:
   ```sql
   CREATE DATABASE bookstore;
   ```

## Backend Setup

1. **Navigate to the Backend directory**:

   ```bash
   cd Backend
   ```

2. **Update appsettings.Development.json** with your MySQL connection:

   ```json
   {
      "Logging": {
         "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
         }
      },
      "ConnectionStrings": {
         "DefaultConnection": "server=localhost;port=3306;database=bookstore;user=root;password=YOUR_MYSQL_PASSWORD"
      },
      "Jwt": {
         "Key": "VeryLongAndSecureKeyForJwt12458910-this-should-be-at-least-32-characters-long",
         "Issuer": "BookstoreApi",
         "Audience": "BookstoreClient"
      }
   }
   ```

   Make sure to replace `YOUR_MYSQL_PASSWORD` with your actual MySQL password.

3. **Start the backend**:

   ```bash
   dotnet run --urls="http://localhost:8080"
   ```

4. The backend API will be available at `http://localhost:8080`

## Frontend Setup

1. **Navigate to the frontend directory**:

   ```bash
   cd frontend
   ```

2. **Install dependencies**:

   ```bash
   npm install
   ```

3. **Create a `.env` file** (optional) with the following content:

   ```
   VITE_API_URL=http://localhost:8080
   ```

4. **Start the development server**:

   ```bash
   npm run dev
   ```

5. The frontend will be available at `http://localhost:4000`

## Testing the Application

1. Make sure both backend and frontend servers are running
2. Open your browser and navigate to `http://localhost:4000`
3. Register a new user account or use the default test account:
   -  Email: `admin@example.com`
   -  Password: `Password123!`

## Unit Testing

1. **Backend Tests**: Navigate to the `Backend.Test` directory and run:

   ```bash
   dotnet test
   ```

2. **Frontend Tests**: Navigate to the `frontend` directory and run:

   ```bash
   npm run test
   ```

## Troubleshooting

-  **Backend Connection Issues**: Ensure your MySQL server is running and the
   connection string in `appsettings.Development.json` is correct
-  **Frontend API Connectivity**: Check that the backend is running on port 8080
   and that your `.env` file has the correct API URL
-  **Image Upload Problems**: Ensure the `wwwroot/uploads` directory exists and
   has write permissions

## Known Issues

-  When updating a book with a large image, you may encounter errors. Try using
   smaller images or avoid changing the image when updating book details.
