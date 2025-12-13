using Microsoft.Extensions.DependencyInjection;
using MediatR;
using AppointmentBooking.Application.Doctors.Handlers;

namespace AppointmentBooking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(CreateDoctorCommandHandler).Assembly));
            return services;
        }
    }
}