# Use .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Copy project files
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Final stage, use .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

# Copy built app
COPY --from=build /app/out .

# Expose port
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "UrlSamurai.dll"]
