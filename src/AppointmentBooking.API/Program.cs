using AppointmentBooking.Application;
using AppointmentBooking.Infrastructure;
using AppointmentBooking.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
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

// Apply EF Migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("Applying database migrations...");
        var context = services.GetRequiredService<EntityContext>();
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");

        var seedingService = services.GetRequiredService<DataSeedingService>();
        await seedingService.SeedDataAsync();
        logger.LogInformation("Data seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
        throw;
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
