namespace BlazorECommerce.Client.Services.AuthenticationServices
{
    public interface IAuthenticationServices
    {
        List<User> Users { get; set; }
        Task<ServiceResponse<int>> Register(UserViewModel usermodel);
        Task<ServiceResponse<string>> Login(UserLoginDTO userLoginDTO);
        Task<ServiceResponse<bool>> ChangePassword(ChangePassword changPassword);
    }
}
