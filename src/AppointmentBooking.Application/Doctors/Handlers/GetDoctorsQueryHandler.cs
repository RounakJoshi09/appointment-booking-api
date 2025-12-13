using AppointmentBooking.Application.Doctors.Queries;
using AppointmentBooking.Application.DTOs.Doctors;
using MediatR;
using AppointmentBooking.Application.Interfaces.Doctors;

namespace AppointmentBooking.Application.Doctors.Handlers;

public class GetDoctorsQueryHandler : IRequestHandler<GetDoctorsQuery, List<DoctorResponse>>
{
    private readonly IDoctorReadRepository _doctorReadRepository;
    public GetDoctorsQueryHandler(IDoctorReadRepository doctorReadRepository)
    {
        _doctorReadRepository = doctorReadRepository;
    }
    public async Task<List<DoctorResponse>> Handle(GetDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await _doctorReadRepository.GetAllDoctors();
        return doctors.Select(doctor => new DoctorResponse(doctor.Id, doctor.Name, doctor.Email)).ToList() ?? [];
    }
}