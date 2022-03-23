using BlazorECommerce.Shared.Models;

namespace BlazorECommerce.Shared
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public Roles Roles { get; set; }
        public int RolesId { get; set; }
    }
}
