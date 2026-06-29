using FlightReservations.Data;
using FlightReservations.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightReservations.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly ApplicationDbContext _db;

        public FlightRepository(ApplicationDbContext db) => _db = db;

        public void Create(Flight flight) => _db.Flights.Add(flight);

        public async Task<IEnumerable<Flight>> GetAllAsync() => await _db.Flights.OrderBy(f => f.DepartureDate).ToListAsync();

        public async Task<Flight?> GetByIdAsync(int id) => await _db.Flights.FindAsync(id);

        public async Task<IEnumerable<Flight>> SearchAsync(City origin, City destination, bool directOnly)
        {
            var query = _db.Flights
                .Where(f => f.Status == FlightStatus.Active && 
                f.Origin == origin 
                && f.Destination == destination).AsQueryable();

            if (directOnly) query = query.Where(f => f.NumberOfStops == 0);

            return await query.ToListAsync();
        }
    }
}
