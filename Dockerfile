# Use .NET 8 SDK to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project file first (improves caching)
COPY Team2_EarthquakeAlertApp.csproj ./

# Restore using the specific project file
RUN dotnet restore Team2_EarthquakeAlertApp.csproj

# Copy the rest of the source code
COPY . .

# Publish the app
RUN dotnet publish Team2_EarthquakeAlertApp.csproj -c Release -o /app --self-contained false

# Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published app
COPY --from=build /app .

# Expose Render port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the app
ENTRYPOINT ["dotnet", "Team2_EarthquakeAlertApp.dll"]
