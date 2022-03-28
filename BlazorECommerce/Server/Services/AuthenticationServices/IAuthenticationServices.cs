namespace BlazorECommerce.Server.Services.AuthenticationServices
{
    public interface IAuthenticationServices
    {
        Task<ServiceResponse<int>> Register(UserViewModel usermodel);
        Task<ServiceResponse<string>> Login(UserLoginDTO usermodel);

        Task<bool> UserExists(string email, string username);
        Task<ServiceResponse<bool>> ChangePassword(int userid, string password);
    }
}
