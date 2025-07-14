## Running the Project with Docker

This project provides Dockerfiles for both the backend (C#/.NET) and frontend (TypeScript/Vue) applications, as well as a `docker-compose.yml` to orchestrate the backend, frontend, and MySQL database services.

### Project-Specific Requirements

- **Backend**: .NET 8.0 (as specified in the Dockerfile and `backend.csproj`)
- **Frontend**: Node.js 22.13.1 (as specified in the Dockerfile)
- **Database**: MySQL (latest)

### Required Environment Variables

The backend service requires several environment variables for proper operation:

- `ASPNETCORE_ENVIRONMENT` (default: `Production`)
- `DATABASE_URL` (connection string for MySQL, e.g., `server=mysql-db;port=3306;database=bookstore;user=root;password=example`)
- `JWT_SECRET` (a secure, 32+ character string for JWT authentication)
- `JWT_ISSUER` (e.g., `BookstoreApi`)
- `JWT_AUDIENCE` (e.g., `BookstoreClient`)

Set these in a `.env` file or via your deployment platform. The compose file includes commented examples for reference.

The frontend can also use a `.env` file for configuration if needed.

### Build and Run Instructions

1. **Clone the repository** and ensure Docker and Docker Compose are installed.
2. **(Optional)** Create a `.env` file in the `./Backend` directory with the required environment variables for the backend.
3. **(Optional)** Create a `.env` file in the `./frontend` directory for frontend configuration if needed.
4. **Build and start all services:**

   ```sh
   docker compose up --build
   ```

   This will build and start the backend, frontend, and MySQL database containers.

### Special Configuration Notes

- The backend expects a MySQL database and uses environment variables for connection and JWT configuration.
- The backend's uploads directory (`/app/wwwroot/uploads`) is created and made writable by the Dockerfile.
- The MySQL root password and user credentials are set in the compose file. **Change these for production deployments!**
- Database data is not persisted by default. Uncomment the `volumes` section in the compose file to enable data persistence.
- The frontend is served as a static site using the `serve` package on port 4000.
- The frontend expects the backend API to be available at `http://localhost:8080` (adjust as needed for your deployment).

### Ports Exposed

- **Backend (csharp-backend):** `8080` (API and Swagger UI)
- **Frontend (typescript-frontend):** `4000` (static site)
- **MySQL (mysql-db):** `3306` (for local development; remove or restrict in production)

### Example `.env` for Backend

```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=server=mysql-db;port=3306;database=bookstore;user=root;password=example
JWT_SECRET=your-super-secret-jwt-key-32-characters
JWT_ISSUER=BookstoreApi
JWT_AUDIENCE=BookstoreClient
```

---

_Refer to the `DEPLOYMENT.md` for more advanced deployment options and details._
