# Use .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything into the container
COPY . .

# Restore and build
RUN dotnet restore
RUN dotnet publish -c Release -o /app --self-contained false

# Use the ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy app from build stage
COPY --from=build /app .

# Expose Render's expected port
EXPOSE 8080

# Configure ASP.NET to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "Team2_EarthquakeAlertApp.dll"]