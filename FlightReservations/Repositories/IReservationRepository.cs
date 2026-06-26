using FlightReservations.Models;

namespace FlightReservations.Repositories
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(int id);
        Task<int> GetReservedSeatsForFlightAsync(int flightId);
        Task<IEnumerable<Reservation>> GetForVisitorAsync(string visitorId);
        Task<IEnumerable<Reservation>> GetPendingAsync();
        Task<IEnumerable<Reservation>> GetForFlightAsync(int flightId);
        void Create(Reservation reservation);
    }
}
