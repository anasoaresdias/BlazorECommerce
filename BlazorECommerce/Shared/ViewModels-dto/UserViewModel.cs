using BlazorECommerce.Shared.Models;
using System.ComponentModel.DataAnnotations;

namespace BlazorECommerce.Shared.ViewModels_dto
{
    public class UserViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassowrd { get; set; } = string.Empty;
        public Roles? Roles { get; set; }

        [Required]
        public int RolesId { get; set; }
    }
}
