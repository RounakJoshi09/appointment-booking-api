using AppointmentBooking.Infrastructure.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AppointmentBooking.Application.Interfaces;

namespace AppointmentBooking.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EntityContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(EntityContext).Assembly.FullName)));

            services.AddScoped<IEntityContext>(provider => provider.GetRequiredService<EntityContext>());

            return services;
        }
    }
}