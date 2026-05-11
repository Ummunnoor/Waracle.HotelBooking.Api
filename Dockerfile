# Use the official .NET 9.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file and project files
COPY ["Waracle.HotelBooking.sln", "."]
COPY ["src/Waracle.HotelBooking.Api/Waracle.HotelBooking.Api.csproj", "src/Waracle.HotelBooking.Api/"]

# Restore dependencies
RUN dotnet restore "src/Waracle.HotelBooking.Api/Waracle.HotelBooking.Api.csproj"

# Copy the rest of the source code
COPY ["src/Waracle.HotelBooking.Api/", "src/Waracle.HotelBooking.Api/"]

# Build the application
WORKDIR "/src/src/Waracle.HotelBooking.Api"
RUN dotnet build "Waracle.HotelBooking.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Waracle.HotelBooking.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 9.0 runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "Waracle.HotelBooking.Api.dll"]