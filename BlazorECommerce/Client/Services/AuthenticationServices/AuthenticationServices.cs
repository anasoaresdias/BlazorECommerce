using System.Net.Http.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;

namespace BlazorECommerce.Client.Services.AuthenticationServices
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly HttpClient http;
        private readonly AuthenticationStateProvider authenticationState;
        private readonly ILocalStorageService localStorage;

        public AuthenticationServices(HttpClient http, AuthenticationStateProvider authenticationState, ILocalStorageService localStorage)
        {
            this.http = http;
            this.authenticationState = authenticationState;
            this.localStorage = localStorage;
        }

        public List<User> Users { get; set; } = new List<User>();

        public async Task<ServiceResponse<int>> Register(UserViewModel usermodel)
        {
            var result = await http.PostAsJsonAsync("api/Authentication/register", usermodel);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<int>>();
        }

        public async Task<ServiceResponse<string>> Login(UserLoginDTO userLoginDTO)
        {
            var result = await http.PostAsJsonAsync("api/Authentication/login", userLoginDTO);
            var token = await result.Content.ReadFromJsonAsync<ServiceResponse<string>>();
            await localStorage.SetItemAsync("token",token.Data);
            await authenticationState.GetAuthenticationStateAsync();
            return new ServiceResponse<string>();
        }

        public async Task<ServiceResponse<bool>> ChangePassword(ChangePassword changePassword)
        {
            var result = await http.PostAsJsonAsync("api/Authentication/change-password", changePassword.Password);
            return await result.Content.ReadFromJsonAsync<ServiceResponse<bool>>();
        }
    }
}
