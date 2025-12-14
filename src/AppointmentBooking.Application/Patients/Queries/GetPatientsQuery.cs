using MediatR;
using AppointmentBooking.Application.DTOs.Patients;

namespace AppointmentBooking.Application.Patients.Queries;

public record GetPatientsQuery : IRequest<List<PatientResponse>>;
