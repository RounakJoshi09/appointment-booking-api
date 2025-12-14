using System.ComponentModel.DataAnnotations;
using AppointmentBooking.Application.Appointments.Commands;
using AppointmentBooking.Application.Appointments.Handlers;
using AppointmentBooking.Application.DTOs.Appointments;
using AppointmentBooking.Application.Interfaces.Appointments;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace AppointmentBooking.Application.Tests;

public class CreateAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _mockRepository;
    private readonly CreateAppointmentCommandHandler _handler;

    public CreateAppointmentCommandHandlerTests()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _handler = new CreateAppointmentCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Outside_Working_Hours_Before()
    {
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 8, 0, 0), // 08:00 IST = 02:30 UTC
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var schedule = new DoctorSchedule
        {
            DoctorId = request.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.PatientExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("within doctor's working hours");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Outside_Working_Hours_After()
    {
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 23, 0, 0),
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var schedule = new DoctorSchedule
        {
            DoctorId = request.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.PatientExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("within doctor's working hours");
    }

    [Fact]
    public async Task Handle_Should_Accept_Appointment_In_Morning_Slot_Of_Multiple_Schedules()
    {
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var request = new CreateAppointmentRequest
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDateTime = new DateTime(2025, 12, 15, 10, 0, 0),
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var morningSchedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(4),
            EndTime = TimeSpan.FromHours(8)
        };

        var eveningSchedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(13),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.PatientExists(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(doctorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(doctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { morningSchedule, eveningSchedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(x => x.CreateAppointment(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken ct) =>
            {
                a.Id = Guid.NewGuid();
                a.CreatedAt = DateTime.UtcNow;
                return a;
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.PatientId.Should().Be(patientId);
        result.DoctorId.Should().Be(doctorId);
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Overlaps()
    {
        var request = new CreateAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            DoctorId = Guid.NewGuid(),
            AppointmentDateTime = new DateTime(2025, 12, 15, 14, 30, 0),
            DurationInMinutes = 30
        };
        var command = new CreateAppointmentCommand(request);

        var schedule = new DoctorSchedule
        {
            DoctorId = request.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(4),
            EndTime = TimeSpan.FromHours(12)
        };

        _mockRepository.Setup(x => x.PatientExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("already has an appointment");
        exception.Message.Should().Contain("choose a different time slot");
    }

    [Fact]
    public async Task Handle_Should_Use_Default_30_Minutes_Duration_When_Not_Specified()
    {
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var request = new CreateAppointmentRequest
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDateTime = new DateTime(2025, 12, 15, 14, 30, 0),
            DurationInMinutes = null
        };
        var command = new CreateAppointmentCommand(request);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(4),
            EndTime = TimeSpan.FromHours(12)
        };

        _mockRepository.Setup(x => x.PatientExists(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(doctorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(doctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(x => x.CreateAppointment(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken ct) =>
            {
                a.Id = Guid.NewGuid();
                a.CreatedAt = DateTime.UtcNow;
                return a;
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Duration.Should().Be(TimeSpan.FromMinutes(30));
    }

    [Fact]
    public async Task Handle_Should_Create_Appointment_Successfully()
    {
        var patientId = Guid.NewGuid();
        var doctorId = Guid.NewGuid();
        var appointmentId = Guid.NewGuid();
        var request = new CreateAppointmentRequest
        {
            PatientId = patientId,
            DoctorId = doctorId,
            AppointmentDateTime = new DateTime(2025, 12, 15, 14, 30, 0),
            DurationInMinutes = 45
        };
        var command = new CreateAppointmentCommand(request);

        var schedule = new DoctorSchedule
        {
            DoctorId = doctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(4),
            EndTime = TimeSpan.FromHours(12)
        };

        _mockRepository.Setup(x => x.PatientExists(patientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.DoctorExists(doctorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _mockRepository.Setup(x => x.GetDoctorSchedules(doctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(doctorId, It.IsAny<DateTime>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(x => x.CreateAppointment(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken ct) =>
            {
                a.Id = appointmentId;
                a.CreatedAt = DateTime.UtcNow;
                return a;
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(appointmentId);
        result.PatientId.Should().Be(patientId);
        result.DoctorId.Should().Be(doctorId);
        result.Duration.Should().Be(TimeSpan.FromMinutes(45));
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        result.AppointmentDateTime.Kind.Should().Be(DateTimeKind.Utc);

        _mockRepository.Verify(x => x.CreateAppointment(
            It.Is<Appointment>(a =>
                a.PatientId == patientId &&
                a.DoctorId == doctorId &&
                a.Duration == TimeSpan.FromMinutes(45) &&
                a.Status == AppointmentStatus.Scheduled),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
