using System.ComponentModel.DataAnnotations;

namespace AppointmentBooking.Domain.Entities;

public class DataMigrationHistory
{
    [Key]
    [MaxLength(255)]
    public required string ScriptName { get; set; }

    public DateTime ExecutedAt { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}
