using MediatR;
using AppointmentBooking.Application.DTOs.Doctors;
namespace AppointmentBooking.Application.Doctors.Queries;

public record GetDoctorsQuery : IRequest<List<DoctorResponse>>;