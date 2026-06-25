using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlightReservations.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int FlightId { get; set; }
        [ForeignKey(nameof(FlightId))]
        public Flight Flight { get; set; } = null!;
        [Required]
        public string VisitorId { get; set; } = null!;
        [ForeignKey(nameof(VisitorId))]
        public string? ApprovedById { get; set; }
        [ForeignKey(nameof(ApprovedById))]
        public ApplicationUser? ApprovedBy { get; set; }
        public ApplicationUser Visitor { get; set; } = null!;
        [Range(1, 1000)]
        public int NumberOfSeats { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
