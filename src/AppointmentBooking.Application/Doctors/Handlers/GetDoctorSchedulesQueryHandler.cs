using MediatR;
using AppointmentBooking.Application.Doctors.Queries;
using AppointmentBooking.Application.DTOs;
using AppointmentBooking.Application.Interfaces.Doctors;

namespace AppointmentBooking.Application.Doctors.Handlers;

public class GetDoctorSchedulesQueryHandler : IRequestHandler<GetDoctorSchedulesQuery, List<DoctorScheduleResponse>>
{
    private readonly IDoctorReadRepository _doctorReadRepository;

    public GetDoctorSchedulesQueryHandler(IDoctorReadRepository doctorReadRepository)
    {
        _doctorReadRepository = doctorReadRepository;
    }

    public async Task<List<DoctorScheduleResponse>> Handle(GetDoctorSchedulesQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _doctorReadRepository.GetDoctorSchedulesByDateIncludingOffDays(request.DoctorId, request.Date);

        return schedules.Select(s => new DoctorScheduleResponse(
            s.Id,
            s.DoctorId,
            s.DayOfWeek,
            s.Date,
            s.StartTime,
            s.EndTime,
            s.IsOffDay
        )).ToList();
    }
}
