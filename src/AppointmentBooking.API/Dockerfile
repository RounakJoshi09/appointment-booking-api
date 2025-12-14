# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file
COPY ["AppointmentBooking.sln", "./"]

# Copy project files
COPY ["src/AppointmentBooking.API/AppointmentBooking.API.csproj", "src/AppointmentBooking.API/"]
COPY ["src/AppointmentBooking.Application/AppointmentBooking.Application.csproj", "src/AppointmentBooking.Application/"]
COPY ["src/AppointmentBooking.Domain/AppointmentBooking.Domain.csproj", "src/AppointmentBooking.Domain/"]
COPY ["src/AppointmentBooking.Infrastructure/AppointmentBooking.Infrastructure.csproj", "src/AppointmentBooking.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/AppointmentBooking.API/AppointmentBooking.API.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/src/AppointmentBooking.API"
RUN dotnet build "AppointmentBooking.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AppointmentBooking.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentBooking.API.dll"]
