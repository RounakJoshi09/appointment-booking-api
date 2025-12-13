using MediatR;
using AppointmentBooking.Application.Doctors.Commands;
using AppointmentBooking.Domain.Entities;
using AppointmentBooking.Application.Interfaces.Doctors;
namespace AppointmentBooking.Application.Doctors.Handlers;

public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, bool>
{
    private readonly IDoctorRepository _doctorRepository;
    public CreateDoctorCommandHandler(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }
    public async Task<bool> Handle(CreateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = new Doctor { Name = request.Request.Name, Email = request.Request.Email };
        await _doctorRepository.CreateDoctor(doctor, cancellationToken);
        return true;
    }
}