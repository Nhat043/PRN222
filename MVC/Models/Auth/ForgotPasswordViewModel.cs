using System.ComponentModel.DataAnnotations;
namespace MVC.Models.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}