using Microsoft.AspNetCore.Components;

namespace BlazorECommerce.Client.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient http;
        private readonly AuthenticationStateProvider authenticationState;
        private readonly NavigationManager navigationManager;

        public OrderService(HttpClient http, AuthenticationStateProvider authenticationState, NavigationManager navigationManager)
        {
            this.http = http;
            this.authenticationState = authenticationState;
            this.navigationManager = navigationManager;
        }

        private async Task<bool> IsUserAuthenticated() => (await authenticationState.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;

        public async Task PlaceOrder()
        {
            if(await IsUserAuthenticated())
            {
                await http.PostAsync("api/order", null);
            }
            else
            {
                navigationManager.NavigateTo("login");
            }
        }
    }
}
