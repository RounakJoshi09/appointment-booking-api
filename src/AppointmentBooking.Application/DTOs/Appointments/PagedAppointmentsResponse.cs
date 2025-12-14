namespace AppointmentBooking.Application.DTOs.Appointments;

public record PagedAppointmentsResponse(
    List<AppointmentResponse> Appointments,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);
