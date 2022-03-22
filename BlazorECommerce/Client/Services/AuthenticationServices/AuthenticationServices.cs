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

        public async Task Register(UserViewModel usermodel)
        {
            await http.PostAsJsonAsync("api/Authentication/register", usermodel);
        }

        public async Task Login(UserViewModel usermodel)
        {
            var result = await http.PostAsJsonAsync("api/Authentication/login", usermodel);
            var token = await result.Content.ReadAsStringAsync();
            await localStorage.SetItemAsync("token", token);
            await authenticationState.GetAuthenticationStateAsync();
        }
    }
}
