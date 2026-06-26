using FlightReservations.Models;
using FlightReservations.Repositories;
using System.Data;

namespace FlightReservations.Services
{
    public class ReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FlightService _flightService;
        public ReservationService(IUnitOfWork unitOfWork, FlightService flightService)
        {
            _unitOfWork = unitOfWork;
            _flightService = flightService;
        }
        public async Task<(bool Success, string? Error)> CreateReservationAsync(
            int flightId, string visitorId, int numberOfSeats)
        {
            var flight = await _unitOfWork.Flights.GetByIdAsync(flightId);

            if (flight == null || flight.Status != FlightStatus.Active) return (false, "Flight is not available!");

            if (flight.DepartureDate < DateTime.Now.AddDays(3)) return (false, "Cannot reserve a flight less than 3 days before deporture");

            if (numberOfSeats < 1) return (false, "Number of seats must be at least 1");

            await _unitOfWork.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                int available = await _flightService.GetAvailableSeatsAsync(flightId);
                if(numberOfSeats > available)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return (false, $"Only {available} seats available");
                }

                _unitOfWork.Reservations.Create(new Reservation
                {
                    FlightId = flightId,
                    VisitorId = visitorId,
                    NumberOfSeats = numberOfSeats,
                    Status = ReservationStatus.Pending,
                    CreatedAt = DateTime.Now
                });

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return (true, null);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                return (false, "Could not complete the reservation duo to a conflict. Please try agan.");
            }
        }
        public async Task<bool> ApproveReservationAsync(int reservationId, string agentId)
        {
            var reservation = await _unitOfWork.Reservations.GetByIdAsync(reservationId);
            if (reservation == null || reservation.Status != ReservationStatus.Pending) return false;

            reservation.Status = ReservationStatus.Approved;
            reservation.ApprovedById = agentId;
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<IEnumerable<Reservation>> GetReservationForVisitorAsync(string visitorId)
            => await _unitOfWork.Reservations.GetForVisitorAsync(visitorId);
        public async Task<IEnumerable<Reservation>> GetPendingReservationsAsync()
            => await _unitOfWork.Reservations.GetPendingAsync();
    }
}
