# 🚀 Backend Deployment Guide

## Prerequisites

-  .NET 8.0 SDK installed
-  Docker installed (for containerized deployment)
-  Database (MySQL) hosted separately

## Quick Start - Railway (Recommended)

### 1. Install Railway CLI

```bash
npm install -g @railway/cli
# or
curl -fsSL https://railway.app/install.sh | sh
```

### 2. Login and Deploy

```bash
railway login
railway link  # Link to existing project or create new one
railway up    # Deploy your backend
```

### 3. Environment Variables

Set these in Railway dashboard:

-  `DATABASE_URL`: Your MySQL connection string
-  `JWT_SECRET`: Strong secret key (32+ characters)
-  `JWT_ISSUER`: BookstoreApi
-  `JWT_AUDIENCE`: BookstoreClient
-  `ASPNETCORE_ENVIRONMENT`: Production

## Alternative Hosting Options

### 🔷 Azure App Service

1. **Create Azure resources:**

```bash
az group create --name bookstore-rg --location "East US"
az appservice plan create --name bookstore-plan --resource-group bookstore-rg --sku B1
az webapp create --resource-group bookstore-rg --plan bookstore-plan --name your-bookstore-api
```

2. **Deploy using Visual Studio or CLI:**

```bash
dotnet publish -c Release
# Use Azure DevOps or GitHub Actions for CI/CD
```

### 🟢 Heroku

1. **Install Heroku CLI and login:**

```bash
heroku create your-bookstore-api
heroku container:login
```

2. **Deploy with Docker:**

```bash
heroku container:push web
heroku container:release web
```

### 🟠 AWS Elastic Beanstalk

1. **Install EB CLI:**

```bash
pip install awsebcli
```

2. **Initialize and deploy:**

```bash
eb init
eb create production
eb deploy
```

### 🟡 DigitalOcean App Platform

1. Connect your GitHub repository
2. Select "Dockerfile" as build method
3. Set environment variables
4. Deploy

## Database Hosting Options

### 🗄️ Managed Database Services:

-  **Railway Database**: Built-in MySQL
-  **Azure Database for MySQL**
-  **AWS RDS MySQL**
-  **PlanetScale** (MySQL-compatible)
-  **DigitalOcean Managed Databases**

### Connection String Format:

```
server=your-host;port=3306;database=bookstore;user=username;password=password;sslmode=required
```

## Environment Variables Required

```bash
# Database
DATABASE_URL=server=host;port=3306;database=bookstore;user=username;password=password

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-at-least-32-characters-long
JWT_ISSUER=BookstoreApi
JWT_AUDIENCE=BookstoreClient

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://*:$PORT
```

## Pre-deployment Checklist

-  [ ] All tests passing (`dotnet test`)
-  [ ] Production appsettings.json configured
-  [ ] Database migrations ready
-  [ ] Environment variables set
-  [ ] CORS configured for frontend domain
-  [ ] Rate limiting configured
-  [ ] Image upload directory created

## Post-deployment Steps

1. **Run database migrations:**

```bash
# If using EF Core migrations
dotnet ef database update --connection "your-connection-string"
```

2. **Test endpoints:**

```bash
curl https://your-api-domain.com/health
curl https://your-api-domain.com/api/books
```

3. **Monitor logs:**
   -  Railway: `railway logs`
   -  Azure: Azure Portal
   -  Heroku: `heroku logs --tail`

## Security Considerations

1. **Environment Variables**: Never commit secrets to git
2. **HTTPS**: Ensure SSL/TLS is enabled
3. **CORS**: Configure for your frontend domain only
4. **Rate Limiting**: Already configured in your app
5. **Input Validation**: Already implemented with DataAnnotations

## Performance Optimization

1. **Database Connection Pooling**: Already configured
2. **Static File Caching**: Already configured
3. **Image Optimization**: Using ImageSharp
4. **Response Compression**: Consider adding in Startup.cs

## Monitoring and Logging

-  Use Application Insights (Azure)
-  Railway provides built-in monitoring
-  Consider adding health checks endpoint
-  Monitor database performance

## Cost Estimation

### Free Tiers:

-  **Railway**: $5/month for hobby plan
-  **Heroku**: Free tier discontinued
-  **Azure**: Free tier with limitations

### Paid Options:

-  **Railway**: $5-20/month
-  **Azure App Service**: $13+/month
-  **AWS**: Variable based on usage
-  **DigitalOcean**: $5+/month

## Troubleshooting

### Common Issues:

1. **Port binding**: Ensure app listens on `$PORT` environment variable
2. **Database connection**: Check connection string format
3. **CORS errors**: Verify frontend domain in CORS policy
4. **File uploads**: Ensure upload directory exists and has write permissions

### Debugging:

```bash
# Check logs
railway logs
# or
az webapp log tail --name your-app-name --resource-group your-rg

# Test locally with production settings
export ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

For detailed setup of any specific platform, refer to their documentation or ask
for platform-specific guidance.
