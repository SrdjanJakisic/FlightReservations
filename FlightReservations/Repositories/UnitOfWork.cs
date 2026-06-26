using FlightReservations.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace FlightReservations.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        private IDbContextTransaction? _transaction;
        public IFlightRepository Flights { get; }
        public IReservationRepository Reservations { get; }
        public UnitOfWork(ApplicationDbContext db, IFlightRepository flights, IReservationRepository reservations)
        {
            _db = db;
            Flights = flights;
            Reservations = reservations;
        }
        public async Task BeginTransactionAsync(IsolationLevel isolationLevel) 
            => _transaction = await _db.Database.BeginTransactionAsync(isolationLevel);
        public async Task CommitTransactionAsync()
        {
            if (_transaction is null) return;

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task RollbackTransactionAsync()
        {
            if (_transaction is null) return;
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        public async Task<int> SaveChangesAsync() => await _db.SaveChangesAsync();
    }
}
