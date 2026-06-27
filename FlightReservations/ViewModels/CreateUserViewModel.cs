using System.ComponentModel.DataAnnotations;

namespace FlightReservations.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "First name")]
        public string? FirstName { get; set; }
        [Display(Name = "Last name")]
        public string? LastName { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Role { get; set; } = "Visitor";
    }
}
