namespace BlazorECommerce.Server.Services.AuthenticationServices
{
    public interface IAuthenticationServices
    {
        Task<ServiceResponse<User>> Register(UserViewModel usermodel);
        Task<string> Login(UserViewModel usermodel);
    }
}
