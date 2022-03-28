namespace BlazorECommerce.Server.Services.AuthenticationServices
{
    public interface IAuthenticationServices
    {
        Task<ServiceResponse<int>> Register(UserViewModel usermodel);
        Task<ServiceResponse<string>> Login(UserViewModel usermodel);

        Task<bool> UserExists(string email, string username);
    }
}
