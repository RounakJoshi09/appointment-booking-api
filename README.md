# appointment-booking-api

A microservice‑based system providing APIs for appointment booking and management.

Assumptions:

- The doctor follows a recurring weekly schedule.
- The schedule may include weekly offs such as Sunday, Saturday, or any weekday.
- The doctor may take leave on specific individual dates.
- The doctor’s schedule can be modified for particular dates when needed
- Doctors schedule startTime and endTime are stored in UTC time in the database.
- While Getting doctor availability, the startTime and endTime is being converted to the local time zone of the doctor.
- A Doctor can have multiple Schdules for the Same Day

- For Appointment Booking
  - The appointment date time is being expected in clients timezone(considering IST Currently).

## Running Tests

To run all tests:

```bash
dotnet test
```

To run tests for a specific project:

```bash
dotnet test tests/AppointmentBooking.Application.Tests/AppointmentBooking.Application.Tests.csproj
```
