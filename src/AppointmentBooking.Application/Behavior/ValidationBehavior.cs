using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AppointmentBooking.Application.Behavior;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationBehavior(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        if (validator != null)
        {
            validator.Validate(request);
        }
        return await next();
    }
}
