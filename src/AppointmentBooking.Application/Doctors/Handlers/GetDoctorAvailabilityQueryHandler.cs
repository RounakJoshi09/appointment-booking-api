using AppointmentBooking.Application.Constants;
using AppointmentBooking.Application.Doctors.Queries;
using AppointmentBooking.Application.DTOs.Doctors;
using AppointmentBooking.Application.Interfaces.Doctors;
using MediatR;

namespace AppointmentBooking.Application.Doctors.Handlers;

public class GetDoctorAvailabilityQueryHandler : IRequestHandler<GetDoctorAvailabilityQuery, DoctorAvailabilityResponse>
{
    private readonly IDoctorReadRepository _doctorReadRepository;

    public GetDoctorAvailabilityQueryHandler(IDoctorReadRepository doctorReadRepository)
    {
        _doctorReadRepository = doctorReadRepository;
    }

    public async Task<DoctorAvailabilityResponse> Handle(GetDoctorAvailabilityQuery request, CancellationToken cancellationToken)
    {
        var schedules = await _doctorReadRepository.GetDoctorSchedulesByDate(request.DoctorId, request.Date);

        var timeSlots = schedules
            .Select(schedule => new TimeSlot(
                Utils.ConvertUtcToTimeZone(schedule.StartTime!.Value, TimeZoneConstants.IndiaStandardTime),
                Utils.ConvertUtcToTimeZone(schedule.EndTime!.Value, TimeZoneConstants.IndiaStandardTime)
            ))
            .OrderBy(slot => slot.StartTime)
            .ToList();

        return new DoctorAvailabilityResponse(
            request.DoctorId,
            request.Date,
            timeSlots
        );
    }
}
