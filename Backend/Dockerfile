# Use the official .NET 8.0 runtime as base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy only the project file first to restore dependencies
COPY ["Backend/backend.csproj", "Backend/"]
RUN dotnet restore "Backend/backend.csproj" --disable-parallel


# Now copy the rest of the code
COPY . .
WORKDIR "/src/Backend"
RUN dotnet build "backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "backend.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create uploads directory
RUN mkdir -p /app/wwwroot/uploads

# Switch to non-root user for better security
USER $APP_UID

ENTRYPOINT ["dotnet", "backend.dll"]
