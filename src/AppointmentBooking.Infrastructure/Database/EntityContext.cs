using System.Reflection;
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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DataMigrationHistory> DataMigrationHistory { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}