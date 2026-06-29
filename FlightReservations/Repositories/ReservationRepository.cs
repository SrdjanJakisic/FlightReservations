using FlightReservations.Data;
using FlightReservations.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservations.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ApplicationDbContext _db;

        public ReservationRepository(ApplicationDbContext db) => _db = db;

        public void Create(Reservation reservation) => _db.Reservations.Add(reservation);

        public async Task<Reservation?> GetByIdAsync(int id) => await _db.Reservations.FindAsync(id);

        public async Task<IEnumerable<Reservation>> GetForFlightAsync(int flightId) => await _db.Reservations
            .Where(r => r.FlightId == flightId && r.Status != ReservationStatus.Cancelled).ToListAsync();

        public async Task<IEnumerable<Reservation>> GetForVisitorAsync(string visitorId)
            => await _db.Reservations.Include(r => r.Flight)
            .Where(r => r.VisitorId == visitorId)
            .OrderBy(r => r.CreatedAt).ToListAsync();

        public async Task<IEnumerable<Reservation>> GetPendingAsync() => await _db.Reservations
            .Include(r => r.Flight)
            .Include(r => r.Visitor)
            .Where(r => r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.CreatedAt).ToListAsync();

        public async Task<int> GetReservedSeatsForFlightAsync(int flightId) => await _db.Reservations
            .Where(r => r.FlightId == flightId && (r.Status == ReservationStatus.Pending || r.Status == ReservationStatus.Approved))
            .SumAsync(r => (int?)r.NumberOfSeats) ?? 0;
    }
}
