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
        var availabilitySlots = await _doctorReadRepository.GetDoctorsAvailabilityByDate(request.DoctorId, request.Date);

        var timeSlots = availabilitySlots
            .Select(slot => new TimeSlot(
                Utils.ConvertUtcToTimeZone(slot.StartTime, TimeZoneConstants.IndiaStandardTime),
                Utils.ConvertUtcToTimeZone(slot.EndTime, TimeZoneConstants.IndiaStandardTime)
            ))
            .ToList();

        return new DoctorAvailabilityResponse(
            request.DoctorId,
            request.Date,
            timeSlots
        );
    }
}
