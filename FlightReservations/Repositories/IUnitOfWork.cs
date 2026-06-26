using System.Data;

namespace FlightReservations.Repositories
{
    public interface IUnitOfWork
    {
        IFlightRepository Flights { get; }
        IReservationRepository Reservations { get; }

        Task<int> SaveChangesAsync();

        Task BeginTransactionAsync(IsolationLevel isolationLevel);
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
