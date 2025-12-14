using AppointmentBooking.Application.Patients.Queries;
using AppointmentBooking.Application.DTOs.Patients;
using MediatR;
using AppointmentBooking.Application.Interfaces.Patients;

namespace AppointmentBooking.Application.Patients.Handlers;

public class GetPatientsQueryHandler : IRequestHandler<GetPatientsQuery, List<PatientResponse>>
{
    private readonly IPatientReadRepository _patientReadRepository;

    public GetPatientsQueryHandler(IPatientReadRepository patientReadRepository)
    {
        _patientReadRepository = patientReadRepository;
    }

    public async Task<List<PatientResponse>> Handle(GetPatientsQuery request, CancellationToken cancellationToken)
    {
        var patients = await _patientReadRepository.GetAllPatients();
        return patients.Select(patient => new PatientResponse(patient.Id, patient.Name, patient.Email)).ToList() ?? [];
    }
}
