using System.ComponentModel.DataAnnotations;
using DAL.Models;

namespace MVC.Models.User
{
    public class ProfileViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Display(Name = "Name")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string? Name { get; set; }

        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [Display(Name = "Address")]
        [StringLength(100, ErrorMessage = "Address cannot exceed 100 characters")]
        public string? Address { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}
