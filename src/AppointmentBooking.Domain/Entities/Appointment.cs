using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppointmentBooking.Domain.Enums;

namespace AppointmentBooking.Domain.Entities;

public class Appointment : AuditEntity
{
    [Required]
    public Guid PatientId { get; set; }

    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    [Required]
    public DateTime AppointmentDateTime { get; set; }

    [Required]
    [Column(TypeName = "time")]
    public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(30);

    [ForeignKey(nameof(PatientId))]
    public virtual Patient? Patient { get; set; }

    [ForeignKey(nameof(DoctorId))]
    public virtual Doctor? Doctor { get; set; }
}