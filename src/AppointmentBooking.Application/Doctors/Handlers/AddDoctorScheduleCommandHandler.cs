using MediatR;
using AppointmentBooking.Application.Doctors.Commands;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Application.Interfaces.Doctors;

namespace AppointmentBooking.Application.Doctors.Handlers;

public class AddDoctorScheduleCommandHandler : IRequestHandler<AddDoctorScheduleCommand, string>
{
    private readonly IDoctorRepository _doctorRepository;

    public AddDoctorScheduleCommandHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<string> Handle(AddDoctorScheduleCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;

        var doctorExists = await _doctorRepository.DoctorExists(req.DoctorId, cancellationToken);
        if (!doctorExists)
        {
            throw new ArgumentException($"Doctor with ID {req.DoctorId} not found.");
        }

        var schedule = new DoctorSchedule
        {
            DoctorId = req.DoctorId,
            DayOfWeek = req.DayOfWeek,
            Date = req.Date,
            StartTime = req.StartTime,
            EndTime = req.EndTime,
            IsOffDay = req.IsOffDay
        };

        var createdSchedule = await _doctorRepository.AddDoctorSchedule(schedule, cancellationToken);

        if (createdSchedule == null)
        {
            throw new InvalidOperationException("Failed to create doctor schedule.");
        }

        return "Doctor schedule added successfully.";
    }
}
