using FlightReservations.Models;

namespace FlightReservations.ViewModels
{
    public class FlightListItemViewModel
    {
        public Flight Flight { get; set; }
        public int AvailableSeats { get; set; }
    }
}
