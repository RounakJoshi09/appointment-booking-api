using MediatR;
using AppointmentBooking.Application.DTOs;
namespace AppointmentBooking.Application.Doctors.Commands;

public record CreateDoctorCommand(CreateDoctorRequest Request) : IRequest<bool>;