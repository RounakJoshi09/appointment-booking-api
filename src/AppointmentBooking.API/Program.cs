using AppointmentBooking.Application;
using AppointmentBooking.Infrastructure;
using AppointmentBooking.Infrastructure.Database;
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
builder.Services.AddApplication();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var seedingService = scope.ServiceProvider.GetRequiredService<DataSeedingService>();
    try
    {
        await seedingService.SeedDataAsync();
    }
    catch (Exception ex)
    {
        throw new Exception("An error occurred while seeding the database", ex);
    }
}

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
