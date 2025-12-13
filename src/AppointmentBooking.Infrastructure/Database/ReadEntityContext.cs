using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace AppointmentBooking.Infrastructure.Database
{
    public class ReadEntityContext : DbContext, IEntityContext
    {
        public ReadEntityContext(DbContextOptions<ReadEntityContext> options) : base(options)
        {

        }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
    }
}