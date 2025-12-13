using Microsoft.Extensions.DependencyInjection;
using MediatR;
using AppointmentBooking.Application.Behavior;
using AppointmentBooking.Application.Doctors.Commands;

namespace AppointmentBooking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));
            services.AddTransient<IValidator<CreateDoctorCommand>, CreateDoctorCommandValidator>();

            return services;
        }
    }
}