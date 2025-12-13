namespace AppointmentBooking.Application.Interfaces
{
    public interface IEntityContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}