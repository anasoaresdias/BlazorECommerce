
using System.ComponentModel.DataAnnotations;

namespace BlazorECommerce.Shared.ViewModels_dto
{
    public class ChangePassword
    {
        [Required, StringLength(50, MinimumLength = 6)]
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "The passwords do not match!")]
        public string ConfirmPassword { get; set; }
    }
}
