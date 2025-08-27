# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file and restore packages
COPY EventTicketing.sln .
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy project files
COPY src/EventTicketing.API/EventTicketing.API.csproj src/EventTicketing.API/

# Restore dependencies
RUN dotnet restore src/EventTicketing.API/EventTicketing.API.csproj

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR /app/src/EventTicketing.API
RUN dotnet publish -c Release -o /app/publish --no-restore

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN addgroup --system --gid 1001 eventticketinggroup && \
    adduser --system --uid 1001 --gid 1001 eventticketinguser

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Change ownership of the app directory
RUN chown -R eventticketinguser:eventticketinggroup /app

# Switch to non-root user
USER eventticketinguser

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "EventTicketing.API.dll"]