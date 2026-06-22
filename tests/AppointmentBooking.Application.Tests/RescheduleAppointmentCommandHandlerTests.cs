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

public class RescheduleAppointmentCommandHandlerTests
{
    private readonly Mock<IAppointmentRepository> _mockRepository;
    private readonly RescheduleAppointmentCommandHandler _handler;

    public RescheduleAppointmentCommandHandlerTests()
    {
        _mockRepository = new Mock<IAppointmentRepository>();
        _handler = new RescheduleAppointmentCommandHandler(_mockRepository.Object);
    }

    private static Appointment CreateScheduledAppointment(Guid? id = null, Guid? doctorId = null)
    {
        return new Appointment
        {
            Id = id ?? Guid.NewGuid(),
            PatientId = Guid.NewGuid(),
            DoctorId = doctorId ?? Guid.NewGuid(),
            AppointmentDateTime = DateTime.UtcNow.AddDays(1),
            Duration = TimeSpan.FromMinutes(30),
            Status = AppointmentStatus.Scheduled
        };
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Not_Found()
    {
        var appointmentId = Guid.NewGuid();
        var command = new RescheduleAppointmentCommand(appointmentId, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0)
        });

        _mockRepository.Setup(x => x.GetAppointmentById(appointmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment?)null);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("does not exist");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Is_Cancelled()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Status = AppointmentStatus.Cancelled;

        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0)
        });

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("cancelled");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Appointment_Is_Completed()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Status = AppointmentStatus.Completed;

        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0)
        });

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("completed");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Outside_Working_Hours()
    {
        var appointment = CreateScheduledAppointment();
        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 8, 0, 0),
            DurationInMinutes = 30
        });

        var schedule = new DoctorSchedule
        {
            DoctorId = appointment.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _mockRepository.Setup(x => x.GetDoctorSchedules(appointment.DoctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("within doctor's working hours");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Overlaps_Another_Appointment()
    {
        var appointment = CreateScheduledAppointment();
        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0),
            DurationInMinutes = 30
        });

        var schedule = new DoctorSchedule
        {
            DoctorId = appointment.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _mockRepository.Setup(x => x.GetDoctorSchedules(appointment.DoctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(
                appointment.DoctorId,
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>(),
                appointment.Id))
            .ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            _handler.Handle(command, CancellationToken.None));
        exception.Message.Should().Contain("already has an appointment");
    }

    [Fact]
    public async Task Handle_Should_Reschedule_Successfully()
    {
        var appointment = CreateScheduledAppointment();
        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0),
            DurationInMinutes = 45
        });

        var schedule = new DoctorSchedule
        {
            DoctorId = appointment.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _mockRepository.Setup(x => x.GetDoctorSchedules(appointment.DoctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(
                appointment.DoctorId,
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>(),
                appointment.Id))
            .ReturnsAsync(false);
        _mockRepository.Setup(x => x.UpdateAppointment(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken _) => a);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be(appointment.Id);
        result.Duration.Should().Be(TimeSpan.FromMinutes(45));
        result.Status.Should().Be(AppointmentStatus.Scheduled);
        result.Message.Should().Contain("rescheduled successfully");
        result.AppointmentDateTime.Kind.Should().Be(DateTimeKind.Utc);

        _mockRepository.Verify(x => x.UpdateAppointment(
            It.Is<Appointment>(a =>
                a.Id == appointment.Id &&
                a.Duration == TimeSpan.FromMinutes(45) &&
                a.Status == AppointmentStatus.Scheduled),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockRepository.Verify(x => x.HasOverlappingAppointment(
            appointment.DoctorId,
            It.IsAny<DateTime>(),
            It.IsAny<TimeSpan>(),
            It.IsAny<CancellationToken>(),
            appointment.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Keep_Existing_Duration_When_Not_Specified()
    {
        var appointment = CreateScheduledAppointment();
        appointment.Duration = TimeSpan.FromMinutes(60);

        var command = new RescheduleAppointmentCommand(appointment.Id, new RescheduleAppointmentRequest
        {
            AppointmentDateTime = new DateTime(2025, 12, 20, 14, 30, 0)
        });

        var schedule = new DoctorSchedule
        {
            DoctorId = appointment.DoctorId,
            IsOffDay = false,
            StartTime = TimeSpan.FromHours(9),
            EndTime = TimeSpan.FromHours(17)
        };

        _mockRepository.Setup(x => x.GetAppointmentById(appointment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(appointment);
        _mockRepository.Setup(x => x.GetDoctorSchedules(appointment.DoctorId, It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DoctorSchedule> { schedule });
        _mockRepository.Setup(x => x.HasOverlappingAppointment(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<TimeSpan>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<Guid?>()))
            .ReturnsAsync(false);
        _mockRepository.Setup(x => x.UpdateAppointment(It.IsAny<Appointment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Appointment a, CancellationToken _) => a);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Duration.Should().Be(TimeSpan.FromMinutes(60));
    }
}
