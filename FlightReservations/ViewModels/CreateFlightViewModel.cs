using FlightReservations.Models;
using System.ComponentModel.DataAnnotations;

namespace FlightReservations.ViewModels
{
    public class CreateFlightViewModel
    {
        [Display(Name = "Origin")]
        public City Origin { get; set; }
        [Display(Name = "Destination")]
        public City Destination { get; set; }
        [Display(Name = "Departure date")]
        public DateTime DepartureDate { get; set; }
        [Range(0, 10)]
        [Display(Name = "Number of stops")]
        public int NumberOfStops { get; set; }
        [Range(0, 1000)]
        [Display(Name = "Total seats")]
        public int TotalSeats { get; set; }
    }
}
