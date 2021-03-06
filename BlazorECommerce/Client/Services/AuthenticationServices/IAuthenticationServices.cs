namespace BlazorECommerce.Client.Services.AuthenticationServices
{
    public interface IAuthenticationServices
    {
        List<User> Users { get; set; }
        Task Register(UserViewModel usermodel);
        Task Login(UserViewModel usermodel);
    }
}
