# appointment-booking-api

# Overview:

A microservice‑based system providing APIs for appointment booking and management.

# Implementation Status

Done

# Architecture:

The project follows a microservices-inspired, layered clean architecture. The API layer is implemented with ASP.NET Core and provides RESTful endpoints for appointment management. It interacts with an application layer responsible for business logic and for orchestrating operations between repositories. The data layer uses Entity Framework Core to connect to a SQL Server database, implementing CQRS-style separation with distinct read and write repositories.
Notably, the system is configured with two database contexts—one for reading and one for writing—which allows easy scaling to separate physical read and write databases simply by changing the connection strings.
Docker is used for containerization, with orchestration handled by Docker Compose for streamlined local development and startup. CI/CD is established with GitHub Actions to automate building, testing, and Docker image creation.

# Tech Stack:

- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- Docker
- GitHub Actions
- Docker Compose
- Clean Architecture
- CQRS

# Setup Steps:

### Using Docker Compose:

**Start services:**

```bash
docker-compose up
```

**Stop services:**

```bash
docker-compose down
```

**Rebuild after code changes:**

```bash
docker-compose up --build
```

### Environment Configuration

The default SQL Server password is `Password123!`. To change it:

1. Update the `SA_PASSWORD` in `docker-compose.yml`
2. Update the connection strings in `docker-compose.yml` to use the new password

## Local Development

### Prerequisites

- .NET 9.0 SDK
- SQL Server (or use Docker)
- Docker & Docker Compose (for containerized deployment)

### Running Locally (without Docker)

1. Update connection strings in `src/AppointmentBooking.API/appsettings.json`
2. Run the API:

```bash
dotnet run --project src/AppointmentBooking.API/AppointmentBooking.API.csproj
```

### Running Tests

To run all tests:

```bash
dotnet test
```

## API Endpoints

```
GET /api/health

GET /api/appointments
GET /api/appointments/{appointmentId}
POST /api/appointments
PUT /api/appointments/{appointmentId}/cancel
PUT /api/appointments/{appointmentId}/reschedule

POST /api/doctors
GET /api/doctors
GET /api/doctors/{doctorId}/availability
POST /api/doctors/schedule
GET /api/doctors/{doctorId}/schedules

GET /api/patients
```

## Assumptions

- The doctor follows a recurring weekly schedule.
- The schedule may include weekly offs such as Sunday, Saturday, or any weekday.
- The doctor may take leave on specific individual dates.
- The doctor's schedule can be modified for particular dates when needed
- Doctors schedule startTime and endTime are stored in UTC time in the database.
- While Getting doctor availability, the startTime and endTime is being converted to the local time zone of the doctor.
- A Doctor can have multiple Schdules for the Same Day

- For Appointment Booking
  - The appointment date time is being expected in clients timezone(considering IST Currently).

## Timezone Considerations

- All the timezones are being stored in UTC in the database.
- While adding appointments, the appointment date time is expected in IST Timezone and being converted and stored to UTC in the database.
- While getting doctor availability, the startTime and endTime is being converted to the IST time zone from UTC Timezone.
- The doctors schedules are being stored in UTC time in the database.

## Future Timezone Enhancements

In the future, the conversion from IST (user's timezone) to UTC can be enhanced. We will store the user's timezone and convert the time received from the client to UTC from users timezone before saving it in the database.
