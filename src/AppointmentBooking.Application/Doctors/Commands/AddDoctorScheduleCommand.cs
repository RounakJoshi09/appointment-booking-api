using MediatR;
using AppointmentBooking.Application.DTOs;

namespace AppointmentBooking.Application.Doctors.Commands;

public record AddDoctorScheduleCommand(AddDoctorScheduleRequest Request) : IRequest<string>;
