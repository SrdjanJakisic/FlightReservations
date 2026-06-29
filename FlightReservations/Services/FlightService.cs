using FlightReservations.Models;
using FlightReservations.Repositories;

namespace FlightReservations.Services
{
    public class FlightService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FlightService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        
        public async Task<int> GetAvailableSeatsAsync(int flightId)
        {
            var flight = await _unitOfWork.Flights.GetByIdAsync(flightId);
            if (flight == null) return 0;
            int reserved = await _unitOfWork.Reservations.GetReservedSeatsForFlightAsync(flightId);

            return flight.TotalSeats - reserved;
        }

        public async Task<IEnumerable<Flight>> SearchAvailableFlightsAsync(City origin, City destination, bool directOnly)
        {
            var flights = await _unitOfWork.Flights.SearchAsync(origin, destination, directOnly);

            var result = new List<Flight>();
            foreach (var flight in flights)
            {
                if (await GetAvailableSeatsAsync(flight.Id) > 0) result.Add(flight);
            }

            return result;
        }

        public async Task<IEnumerable<Flight>> GetAllFlightsAsync() 
            => await _unitOfWork.Flights.GetAllAsync();

        public async Task CreateFlightAsync(Flight flight)
        {
            _unitOfWork.Flights.Create(flight);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CancelFlightAsync(int flightId)
        {
            var flight = await _unitOfWork.Flights.GetByIdAsync(flightId);
            if (flight == null) return;
            flight.Status = FlightStatus.Cancelled;

            var reservations = await _unitOfWork.Reservations.GetForFlightAsync(flightId);
            foreach (var r in reservations) r.Status = ReservationStatus.Cancelled;

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
