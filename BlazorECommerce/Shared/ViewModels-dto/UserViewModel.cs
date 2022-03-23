using BlazorECommerce.Shared.Models;

namespace BlazorECommerce.Shared
{
    public class UserViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Roles? Roles { get; set; }
        public int RolesId { get; set; }
    }
}
