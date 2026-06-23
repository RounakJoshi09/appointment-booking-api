using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Domain.Enums;
using AppointmentBooking.Infrastructure.Database;
using AppointmentBooking.Infrastructure.Database.Appointments;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class AppointmentRepositoryTests : IDisposable
{
    private readonly EntityContext _context;
    private readonly AppointmentRepository _repository;

    public AppointmentRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<EntityContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new EntityContext(options);
        _repository = new AppointmentRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    #region CreateAppointment Tests

    [Fact]
    public async Task CreateAppointment_Should_Add_Appointment_To_Database()
    {
        var appointment = new Appointment
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };

        var result = await _repository.CreateAppointment(appointment, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedAppointment = await _context.Appointments.FindAsync(result.Id);
        savedAppointment.Should().NotBeNull();
    }

    #endregion

    #region HasOverlappingAppointment Tests

    [Fact]
    public async Task HasOverlappingAppointment_Should_Return_False_When_No_Appointments_Exist()
    {
        var doctorId = Guid.NewGuid();
        var appointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, appointmentDateTime, duration, CancellationToken.None);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Return_False_For_BackToBack_Appointments()
    {
        var doctorId = Guid.NewGuid();
        var existingAppointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingAppointment);
        await _context.SaveChangesAsync();

        var newAppointmentDateTime = new DateTime(2025, 12, 15, 10, 30, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeFalse("back-to-back appointments should be allowed");
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Return_True_When_New_Starts_During_Existing()
    {
        var doctorId = Guid.NewGuid();
        var existingAppointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingAppointment);
        await _context.SaveChangesAsync();

        var newAppointmentDateTime = new DateTime(2025, 12, 15, 10, 15, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Return_True_When_New_Completely_Covers_Existing()
    {
        var doctorId = Guid.NewGuid();
        var existingAppointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 30, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingAppointment);
        await _context.SaveChangesAsync();

        var newAppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(90);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Ignore_Cancelled_Appointments()
    {
        var doctorId = Guid.NewGuid();
        var cancelledAppointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Cancelled
        };
        await _context.Appointments.AddAsync(cancelledAppointment);
        await _context.SaveChangesAsync();

        var newAppointmentDateTime = new DateTime(2025, 12, 15, 10, 15, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeFalse("cancelled appointments should be ignored");
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Ignore_Excluded_Appointment()
    {
        var doctorId = Guid.NewGuid();
        var existingAppointment = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingAppointment);
        await _context.SaveChangesAsync();

        var resultWithoutExclude = await _repository.HasOverlappingAppointment(
            doctorId,
            existingAppointment.AppointmentDateTime,
            existingAppointment.Duration,
            CancellationToken.None);

        var resultWithExclude = await _repository.HasOverlappingAppointment(
            doctorId,
            existingAppointment.AppointmentDateTime,
            existingAppointment.Duration,
            CancellationToken.None,
            excludeAppointmentId: existingAppointment.Id);

        resultWithoutExclude.Should().BeTrue();
        resultWithExclude.Should().BeFalse("excluded appointment should not count as overlap");
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Detect_Overlap_When_New_Crosses_Midnight_Into_Next_Day()
    {
        var doctorId = Guid.NewGuid();
        var existingNextDay = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 16, 0, 0, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingNextDay);
        await _context.SaveChangesAsync();

        // 23:45 today for 30 min ends 00:15 tomorrow — overlaps existing 00:00 tomorrow
        var newAppointmentDateTime = new DateTime(2025, 12, 15, 23, 45, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeTrue("new appointment spilling past midnight must conflict with early next-day booking");
    }

    [Fact]
    public async Task HasOverlappingAppointment_Should_Detect_Overlap_When_Existing_Crosses_Midnight_From_Previous_Day()
    {
        var doctorId = Guid.NewGuid();
        var existingPreviousDay = new Appointment
        {
            DoctorId = doctorId,
            PatientId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 14, 23, 50, 0, DateTimeKind.Utc),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
        await _context.Appointments.AddAsync(existingPreviousDay);
        await _context.SaveChangesAsync();

        // Existing ends 00:20 today; new at 00:00 for 30 min overlaps
        var newAppointmentDateTime = new DateTime(2025, 12, 15, 0, 0, 0, DateTimeKind.Utc);
        var duration = TimeSpan.FromMinutes(30);

        var result = await _repository.HasOverlappingAppointment(doctorId, newAppointmentDateTime, duration, CancellationToken.None);

        result.Should().BeTrue("existing appointment spilling from previous day must conflict with early-today booking");
    }

    #endregion

    #region GetDoctorSchedules Tests

    [Fact]
    public async Task GetDoctorSchedules_Should_Prioritize_Specific_Date_Over_Recurring()
    {
        var doctorId = Guid.NewGuid();
        var specificDate = new DateTime(2025, 12, 15); // Monday

        var recurringSchedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            DayOfWeek = DayOfWeek.Monday,
            Date = null,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17),
            IsOffDay = false
        };

        var specificSchedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            Date = specificDate,
            StartTime = TimeSpan.FromHours(10),
            EndTime = TimeSpan.FromHours(14),
            IsOffDay = false
        };

        await _context.DoctorSchedules.AddRangeAsync(recurringSchedule, specificSchedule);
        await _context.SaveChangesAsync();

        var result = await _repository.GetDoctorSchedules(doctorId, specificDate, CancellationToken.None);

        result.Should().HaveCount(1);
        result[0].Date.Should().Be(specificDate);
        result[0].StartTime.Should().Be(TimeSpan.FromHours(10), "specific date should override recurring");
    }

    #endregion
}
