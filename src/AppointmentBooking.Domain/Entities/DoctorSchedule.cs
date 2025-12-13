using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Domain.Entities;

public class DoctorSchedule : AuditEntity
{
    [Required]
    public Guid DoctorId { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public DateTime? Date { get; set; }

    public TimeSpan? StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    [Required]
    public bool IsOffDay { get; set; }
}
