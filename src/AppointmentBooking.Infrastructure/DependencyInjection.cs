using AppointmentBooking.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Application.Interfaces;
using AppointmentBooking.Infrastructure.Database.Doctors;
using AppointmentBooking.Application.Interfaces.Doctors;

namespace AppointmentBooking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // For Now Read and Write are using the same database
            services.AddDbContext<EntityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("WriteConnection"),
                    b => b.MigrationsAssembly(typeof(EntityContext).Assembly.FullName)));
            services.AddDbContext<ReadEntityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("ReadConnection"),
                    b => b.MigrationsAssembly(typeof(ReadEntityContext).Assembly.FullName)));
            services.AddScoped<IEntityContext>(provider => provider.GetRequiredService<EntityContext>());
            services.AddScoped<IEntityContext>(provider => provider.GetRequiredService<ReadEntityContext>());
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IDoctorReadRepository, DoctorReadRepository>();
            services.AddScoped<DataSeedingService>();
            return services;
        }
    }
}