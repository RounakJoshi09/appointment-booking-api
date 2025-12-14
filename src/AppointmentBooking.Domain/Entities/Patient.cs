using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Domain.Entities;

public class Patient : AuditEntity
{
    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Email { get; set; }
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
