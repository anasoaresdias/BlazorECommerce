using System.ComponentModel.DataAnnotations;

namespace BlazorECommerce.Shared.ViewModels_dto
{
    public class UserLoginDTO
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
