using System.ComponentModel.DataAnnotations;

namespace FlightReservations.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public City Origin { get; set; }
        public City Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        [Range(0, 10)]
        public int NumberOfStops { get; set; }
        [Range(1, 1000)]
        public int TotalSeats { get; set; }
        public FlightStatus Status { get; set; } = FlightStatus.Active;
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
