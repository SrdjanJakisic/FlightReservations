using FlightReservations.Models;

namespace FlightReservations.Repositories
{
    public interface IFlightRepository
    {
        Task<Flight?> GetByIdAsync(int id);
        Task<IEnumerable<Flight>> GetAllAsync();
        Task<IEnumerable<Flight>> SearchAsync(City origin, City destination, bool directOnly);
        void Create(Flight flight);
    }
}
