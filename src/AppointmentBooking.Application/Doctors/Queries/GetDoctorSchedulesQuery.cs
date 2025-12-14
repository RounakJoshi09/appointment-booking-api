using MediatR;
using AppointmentBooking.Application.DTOs;

namespace AppointmentBooking.Application.Doctors.Queries;

public record GetDoctorSchedulesQuery(Guid DoctorId, DateOnly Date) : IRequest<List<DoctorScheduleResponse>>;
