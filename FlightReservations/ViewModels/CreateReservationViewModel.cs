using System.ComponentModel.DataAnnotations;

namespace FlightReservations.ViewModels
{
    public class CreateReservationViewModel
    {
        public int FlightId { get; set; }
        public int AvailableSeats { get; set; }
        [Range(0, 1000)]
        [Display(Name = "Number of seats")]
        public int NumberOfSeats { get; set; } = 1;
    }
}
