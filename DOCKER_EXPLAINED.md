# Docker & Docker Compose - Beginner's Guide

## 🐳 What is Docker?

**Docker** is a tool that packages your application and everything it needs to run (code, libraries, dependencies) into a **container**.

### Think of it like this:
- **Without Docker**: You need to install .NET, SQL Server, configure everything on your machine
- **With Docker**: Everything runs in isolated "boxes" (containers) that work the same on any computer

### Key Concepts:

**1. Image** 📦
- A blueprint/template for your application
- Like a recipe that describes what goes into the container
- Example: `mcr.microsoft.com/mssql/server:2022-latest` is Microsoft's SQL Server image

**2. Container** 🎁
- A running instance of an image
- Like a virtual computer running inside your actual computer
- Isolated from your main system

**3. Volume** 💾
- Persistent storage for container data
- When container stops, data in volumes remains
- Without volumes, all data is lost when container stops

**4. Network** 🌐
- Allows containers to talk to each other
- Each container gets its own IP address on the network

---

## 🎼 What is Docker Compose?

**Docker Compose** is a tool to run multiple Docker containers together.

### Why need it?
Your application has:
- An API (your .NET app)
- A database (SQL Server)

Both need to:
- Start together
- Talk to each other
- Work as one system

**Docker Compose** manages this with one simple command: `docker-compose up`

---

## 📄 Your docker-compose.yml File Explained

Let me break down your file **line by line**:

### Overall Structure
```yaml
version: "3.8"           # Docker Compose file format version

services:                # List of containers to run
  sqlserver:             # First service (SQL Server)
  api:                   # Second service (your .NET API)

volumes:                 # Persistent storage definitions
networks:                # Network definitions
```

---

## 🗄️ Service 1: SQL Server Database

```yaml
services:
  sqlserver:
```

This section defines the SQL Server container.

### Line 5: Image
```yaml
    image: mcr.microsoft.com/mssql/server:2022-latest
```
- **What**: Download Microsoft's official SQL Server 2022 image
- **From**: `mcr.microsoft.com` (Microsoft Container Registry)
- **Like**: Downloading SQL Server installer, but pre-packaged
- **`:latest`**: Use the newest version

### Line 6: Container Name
```yaml
    container_name: appointmentbooking-sqlserver
```
- **What**: Give this container a friendly name
- **Why**: Easy to identify in logs and commands
- **Instead of**: Random name like `mssql_abc123`

### Lines 7-10: Environment Variables
```yaml
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Password123!
      - MSSQL_PID=Express
```
**Environment variables** are settings passed to the container.

**`ACCEPT_EULA=Y`**
- Accept SQL Server End User License Agreement
- Required to use SQL Server
- `Y` = Yes, I agree

**`SA_PASSWORD=Password123!`**
- Set the admin password for SQL Server
- `sa` = System Administrator (superuser)
- ⚠️ **WARNING**: This is a weak password for demo only! Never use in production!

**`MSSQL_PID=Express`**
- Use SQL Server Express edition (free version)
- Other options: `Developer`, `Standard`, `Enterprise`

### Lines 11-12: Ports
```yaml
    ports:
      - "1433:1433"
```
**Port mapping** connects your computer to the container.

**Format**: `"HOST_PORT:CONTAINER_PORT"`
- **Left side (1433)**: Port on **your Windows machine**
- **Right side (1433)**: Port **inside the container**

**What this means**:
```
Your Computer (localhost:1433) 
        ↓ forwards to ↓
Container (SQL Server listening on 1433)
```

**In practice**:
- Connect from SQL client: `localhost,1433`
- Docker routes it to the container

### Lines 13-14: Volumes
```yaml
    volumes:
      - sqlserver-data:/var/opt/mssql
```
**Volume mapping** saves data permanently.

**Format**: `"VOLUME_NAME:CONTAINER_PATH"`
- **Left side**: Named volume `sqlserver-data` (stored by Docker)
- **Right side**: Where SQL Server stores data inside container

**Why important**:
```
Without volume:
Container stops → All database data LOST ❌

With volume:
Container stops → Data saved in volume ✅
Restart container → Data restored ✅
```

**Location of data**:
- Windows: `\\wsl$\docker-desktop-data\data\docker\volumes\`
- You don't access it directly; Docker manages it

### Lines 15-20: Health Check
```yaml
    healthcheck:
      test: /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P Password123! -C -Q "SELECT 1" || exit 1
      interval: 10s
      timeout: 3s
      retries: 10
      start_period: 10s
```
**Health check** verifies SQL Server is ready.

**`test`**: Command to check if healthy
- Runs `sqlcmd` (SQL command-line tool)
- `-S localhost`: Connect to local server
- `-U sa`: Username
- `-P Password123!`: Password
- `-Q "SELECT 1"`: Run simple query
- `|| exit 1`: If fails, return error

**`interval: 10s`**: Check every 10 seconds

**`timeout: 3s`**: Each check has 3 seconds to complete

**`retries: 10`**: Try 10 times before giving up

**`start_period: 10s`**: Give 10 seconds initial startup time

**Why important**: API won't start until SQL Server is healthy

### Lines 21-22: Network
```yaml
    networks:
      - appointmentbooking-network
```
- Connect this container to the `appointmentbooking-network`
- Allows API and SQL Server to communicate

---

## 🚀 Service 2: Your .NET API

```yaml
  api:
```

This section defines your application container.

### Lines 25-27: Build Configuration
```yaml
    build:
      context: .
      dockerfile: src/AppointmentBooking.API/Dockerfile
```
**Build** means create a Docker image from source code.

**`context: .`**
- **Context** = folder Docker can access during build
- `.` = current directory (where docker-compose.yml is)
- Docker can access all files in this folder and subfolders

**`dockerfile: src/AppointmentBooking.API/Dockerfile`**
- Path to the Dockerfile (build instructions)
- Docker will follow these instructions to build your app image

### Line 28: Container Name
```yaml
    container_name: appointmentbooking-api
```
- Name your API container
- Easy to find in logs

### Lines 29-33: Environment Variables
```yaml
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__WriteConnection=Server=sqlserver;Database=AppointmentBooking;User Id=sa;Password=Password123!;TrustServerCertificate=True;
      - ConnectionStrings__ReadConnection=Server=sqlserver;Database=AppointmentBooking;User Id=sa;Password=Password123!;TrustServerCertificate=True;
```

**`ASPNETCORE_ENVIRONMENT=Development`**
- Tells ASP.NET Core to run in Development mode
- Enables: Swagger UI, detailed errors, development features

**`ASPNETCORE_URLS=http://+:8080`**
- Make API listen on port 8080 inside container
- `+` means all network interfaces

**`ConnectionStrings__WriteConnection=...`**
- ⚠️ **Important**: Uses `__` (double underscore) for nested configuration
- In C#: `Configuration["ConnectionStrings:WriteConnection"]`
- In Docker: `ConnectionStrings__WriteConnection`

**Breaking down the connection string**:
```
Server=sqlserver        ← Container name (not localhost!)
Database=AppointmentBooking
User Id=sa              ← Admin user
Password=Password123!
TrustServerCertificate=True  ← Allow self-signed certificates
```

**Why `Server=sqlserver` not `localhost`?**
- Containers talk via network using container names
- Docker's DNS resolves `sqlserver` → SQL container's IP
- `localhost` would mean "this container" (wrong!)

### Lines 34-35: Ports
```yaml
    ports:
      - "5000:8080"
```
**Port mapping** for API access.

**Format**: `"5000:8080"`
- **Your computer port**: 5000
- **Container port**: 8080

**What this means**:
```
Browser: http://localhost:5000
        ↓ Docker forwards ↓
Container: API listening on port 8080
```

**Why different ports?**
- Flexibility: Run multiple versions on different ports
- Common pattern: Map 80/443 externally, app uses different port internally

### Lines 36-38: Dependencies
```yaml
    depends_on:
      sqlserver:
        condition: service_healthy
```
**Dependency management** - controls startup order.

**`depends_on:`**: API depends on SQL Server

**`condition: service_healthy`**: 
- Wait for SQL Server's health check to pass
- Don't start API until database is ready

**Startup sequence**:
```
1. Start SQL Server container
2. Wait for health check to pass (up to 10 retries)
3. Once healthy, start API container
```

**Without this**: API might start before SQL Server is ready → connection errors

### Lines 39-40: Network
```yaml
    networks:
      - appointmentbooking-network
```
- Connect API to same network as SQL Server
- Both containers can now communicate

### Line 41: Restart Policy
```yaml
    restart: on-failure
```
**Restart policy** controls automatic restarts.

**Options**:
- `no`: Never restart (default)
- `on-failure`: Restart if crashes
- `always`: Always restart, even after manual stop
- `unless-stopped`: Restart unless manually stopped

**`on-failure`**: 
- If API crashes (exception, error) → Docker restarts it
- If you manually stop → Docker doesn't restart
- Good for development

---

## 💾 Volumes Section

```yaml
volumes:
  sqlserver-data:
```

**Named volume definition**.

**What it does**:
- Creates a named volume called `sqlserver-data`
- Docker manages the storage location
- Data persists even when containers are removed

**Without this line**: You'd get an error "volume not found"

---

## 🌐 Networks Section

```yaml
networks:
  appointmentbooking-network:
    driver: bridge
```

**Network definition**.

**`appointmentbooking-network`**: Custom network name

**`driver: bridge`**: 
- **Bridge** is the default Docker network type
- Creates a private network for containers
- Containers on same bridge can communicate
- Isolated from host network

**How it works**:
```
Your Computer
    │
    ├─ appointmentbooking-network (bridge)
    │      │
    │      ├─ sqlserver (IP: 172.18.0.2)
    │      └─ api (IP: 172.18.0.3)
    │
    └─ Other networks
```

**DNS Resolution**:
- API can reach SQL via `sqlserver` (name)
- Docker resolves name → IP automatically

---

## 🏗️ Dockerfile Explained

Your `Dockerfile` builds the API image. Let's break it down:

### Multi-Stage Build Concept

**Three stages**:
1. **Build stage**: Compile the application
2. **Publish stage**: Package for deployment
3. **Runtime stage**: Run the application

**Why multiple stages?**
- **Smaller final image**: Don't include build tools in production
- **Security**: Less software = smaller attack surface
- **Performance**: Faster deployment

---

### Stage 1: Build

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
```
- **Base image**: .NET SDK 9.0 (has compilers, build tools)
- **`AS build`**: Name this stage "build"

```dockerfile
WORKDIR /src
```
- Set working directory inside container to `/src`
- Like `cd /src` in Linux

```dockerfile
COPY ["AppointmentBooking.sln", "./"]
```
- Copy solution file from **your computer** to **/src in container**

```dockerfile
COPY ["src/AppointmentBooking.API/AppointmentBooking.API.csproj", "src/AppointmentBooking.API/"]
COPY ["src/AppointmentBooking.Application/AppointmentBooking.Application.csproj", "src/AppointmentBooking.Application/"]
COPY ["src/AppointmentBooking.Domain/AppointmentBooking.Domain.csproj", "src/AppointmentBooking.Domain/"]
COPY ["src/AppointmentBooking.Infrastructure/AppointmentBooking.Infrastructure.csproj", "src/AppointmentBooking.Infrastructure/"]
```
- Copy all project files (`.csproj`)
- **Why first?**: Docker caching optimization

**Docker Layer Caching**:
- Each instruction creates a "layer"
- If files haven't changed, Docker reuses cached layer
- Project files rarely change → cache this layer
- Source code changes often → rebuild only necessary layers

```dockerfile
RUN dotnet restore "src/AppointmentBooking.API/AppointmentBooking.API.csproj"
```
- Download NuGet packages (dependencies)
- Like `npm install` or `pip install`

```dockerfile
COPY . .
```
- Copy **everything** else (source code)
- Now we have complete project in container

```dockerfile
WORKDIR "/src/src/AppointmentBooking.API"
```
- Change directory to API project

```dockerfile
RUN dotnet build "AppointmentBooking.API.csproj" -c Release -o /app/build
```
- Compile the application
- `-c Release`: Optimized build
- `-o /app/build`: Output to /app/build folder

---

### Stage 2: Publish

```dockerfile
FROM build AS publish
```
- Continue from `build` stage
- Name this stage "publish"

```dockerfile
RUN dotnet publish "AppointmentBooking.API.csproj" -c Release -o /app/publish /p:UseAppHost=false
```
- Package application for deployment
- Creates DLLs, configuration files, dependencies
- `/p:UseAppHost=false`: Don't create native executable (use `dotnet` command)

---

### Stage 3: Runtime (Final)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
```
- **New base**: ASP.NET Runtime 9.0 (much smaller, no build tools)
- **SDK size**: ~700MB
- **Runtime size**: ~200MB
- Name this stage "final"

```dockerfile
WORKDIR /app
```
- Set working directory to `/app`

```dockerfile
EXPOSE 8080
EXPOSE 8081
```
- **Document** that app listens on ports 8080 and 8081
- **Note**: Doesn't actually open ports (that's done in docker-compose with `ports:`)
- For documentation/clarity only

```dockerfile
COPY --from=publish /app/publish .
```
- **Copy from previous stage**: Get compiled app from `publish` stage
- Source: `/app/publish` in publish stage
- Destination: `.` (current dir = `/app`) in final stage
- **Magic**: Only final stage is kept, build stages discarded

```dockerfile
ENTRYPOINT ["dotnet", "AppointmentBooking.API.dll"]
```
- **Command to run** when container starts
- Like double-clicking an executable
- `dotnet AppointmentBooking.API.dll` starts your API

---

## 🔄 Complete Flow Visualization

### When you run `docker-compose up`:

```
Step 1: Network Creation
├─ Docker creates "appointmentbooking-network"
└─ Bridge network with custom DNS

Step 2: Volume Creation
└─ Docker creates "sqlserver-data" volume

Step 3: Pull/Build Images
├─ SQL Server: Pull from mcr.microsoft.com
└─ API: Build from Dockerfile
    ├─ Stage 1: Build with SDK
    ├─ Stage 2: Publish
    └─ Stage 3: Create final runtime image

Step 4: Start SQL Server Container
├─ Create container from SQL image
├─ Mount sqlserver-data volume
├─ Set environment variables
├─ Connect to network
├─ Start SQL Server process
└─ Begin health checks (every 10s)

Step 5: Wait for SQL Health
└─ API waits until SQL is healthy

Step 6: Start API Container
├─ Create container from built image
├─ Set environment variables
├─ Connect to network
├─ Start API process (dotnet AppointmentBooking.API.dll)
└─ Apply migrations and seed data

Step 7: Services Running
├─ SQL Server: listening on localhost:1433
└─ API: accessible at localhost:5000
```

---

## 🎯 Common Commands Explained

### Start Everything
```powershell
docker-compose up
```
- Reads `docker-compose.yml`
- Creates networks/volumes
- Builds/pulls images
- Starts containers
- Shows logs in terminal

### Start in Background
```powershell
docker-compose up -d
```
- `-d` = detached mode (background)
- Terminal returns immediately
- Containers run in background

### Rebuild and Start
```powershell
docker-compose up --build
```
- Rebuild API image even if it exists
- Use after code changes

### Stop Everything
```powershell
docker-compose down
```
- Stops all containers
- Removes containers
- Removes network
- **Keeps volumes** (data safe!)

### Stop and Remove Volumes (⚠️ Deletes Database!)
```powershell
docker-compose down -v
```
- Stops containers
- **Removes volumes** → All database data lost!
- Use for fresh start

### View Logs
```powershell
docker-compose logs
```
- Show logs from all containers

```powershell
docker-compose logs api
```
- Show only API logs

```powershell
docker-compose logs -f sqlserver
```
- Follow SQL Server logs (live updates)

### List Running Containers
```powershell
docker-compose ps
```
- Shows status of services

### Execute Command in Container
```powershell
docker-compose exec api bash
```
- Open bash shell inside API container
- Like SSH into the container

---

## 🔍 Troubleshooting Tips

### Container won't start
```powershell
# View detailed logs
docker-compose logs

# Check specific service
docker-compose logs sqlserver
```

### Port already in use
```
Error: Bind for 0.0.0.0:5000 failed: port is already allocated
```
**Solution**: Change port in docker-compose.yml
```yaml
ports:
  - "5001:8080"  # Use 5001 instead
```

### SQL Server won't connect
```powershell
# Check if container is running
docker-compose ps

# Check SQL logs
docker-compose logs sqlserver

# Verify health check
docker inspect appointmentbooking-sqlserver | findstr "Health"
```

### Rebuild after code changes
```powershell
docker-compose down
docker-compose up --build
```

### Fresh start (⚠️ deletes database)
```powershell
docker-compose down -v
docker-compose up --build
```

---

## 💡 Key Takeaways

1. **Docker Compose = Orchestra Conductor**
   - Manages multiple containers
   - One command to start/stop everything

2. **Containers are Isolated**
   - Use container names (not localhost) for inter-container communication
   - Example: `Server=sqlserver` not `Server=localhost`

3. **Volumes = Persistent Storage**
   - Data survives container restarts
   - `docker-compose down` keeps volumes
   - `docker-compose down -v` deletes volumes

4. **Networks Enable Communication**
   - Containers on same network can talk
   - Docker DNS resolves container names

5. **Health Checks Prevent Errors**
   - API waits for SQL to be ready
   - Prevents connection failures

6. **Multi-Stage Builds = Smaller Images**
   - Build with SDK (large)
   - Run with runtime (small)
   - Final image only has what's needed

---

## 🎓 Next Steps to Learn

1. **Try it out**:
   ```powershell
   docker-compose up
   # Watch the logs, see the startup sequence
   ```

2. **Experiment**:
   - Change port from 5000 to 5001
   - Modify environment variable
   - See the effects

3. **Inspect**:
   ```powershell
   docker-compose ps          # See running containers
   docker network ls          # See networks
   docker volume ls           # See volumes
   ```

4. **Dive deeper**:
   - Read Docker documentation
   - Try Docker Desktop UI (visual interface)
   - Experiment with other images

---

**Congratulations!** 🎉 You now understand Docker Compose! This knowledge applies to any multi-container application, not just this project.
