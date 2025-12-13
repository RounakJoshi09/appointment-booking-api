using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentBooking.Domain.Entities;

public class AuditEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}