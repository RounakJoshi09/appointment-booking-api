using MediatR;
using AppointmentBooking.Application.DTOs.Doctors;

namespace AppointmentBooking.Application.Doctors.Queries;

public record GetDoctorAvailabilityQuery(
    Guid DoctorId,
    DateOnly Date
) : IRequest<DoctorAvailabilityResponse>;
