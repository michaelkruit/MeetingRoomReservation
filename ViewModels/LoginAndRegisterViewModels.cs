using System.ComponentModel.DataAnnotations;

namespace MeetingRooms.ViewModels
{
    public abstract class LoginAndRegisterViewModel
    {
        [Required]
        [MaxLength(length: 50)]
        public string CompanyName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class LoginViewModel : LoginAndRegisterViewModel { }
    
    public class RegisterViewModel : LoginAndRegisterViewModel { }
}
