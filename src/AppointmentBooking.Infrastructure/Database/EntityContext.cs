using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace AppointmentBooking.Infrastructure.Database
{
    public class EntityContext : DbContext, IEntityContext
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        {

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<AuditEntity>())
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    }
}