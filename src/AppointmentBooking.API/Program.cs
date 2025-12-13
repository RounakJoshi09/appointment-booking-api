using AppointmentBooking.Infrastructure;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Appointment Booking API",
        Version = "v1",
        Description = "API for managing appointment bookings"
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
   {
       options.DisplayRequestDuration();
       options.DocumentTitle = "Appointment Booking API Documentation";
       options.DocExpansion(DocExpansion.None);
   });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
