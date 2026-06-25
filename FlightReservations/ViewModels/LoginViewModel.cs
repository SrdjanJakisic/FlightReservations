using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlightReservations.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; } 
    }
}
