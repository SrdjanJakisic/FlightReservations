using FlightReservations.Models;
using System.ComponentModel.DataAnnotations;

namespace FlightReservations.ViewModels
{
    public class SearchFlightsViewModel
    {
        [Display(Name = "Origin")]
        public City Origin { get; set; }
        [Display(Name = "Destination")]
        public City Destination { get; set; }
        [Display(Name = "Direct flights only")]
        public bool DirectOnly { get; set; }
        public List<FlightListItemViewModel>? Results { get; set; }
    }
}
