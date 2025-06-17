using System.ComponentModel.DataAnnotations;

namespace MVC.Models.Auth
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string OTP { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        public string Email { get; set; }
    }
}