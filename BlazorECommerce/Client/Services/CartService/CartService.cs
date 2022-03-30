
namespace BlazorECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService localStorage;
        private readonly HttpClient http;
        private readonly AuthenticationStateProvider authentication;

        public CartService(ILocalStorageService localStorage, HttpClient http, AuthenticationStateProvider authentication)
        {
            this.localStorage = localStorage;
            this.http = http;
            this.authentication = authentication;
        }

        private async Task<List<CartItem>> GetCartLocalStorage()
        {
            return await localStorage.GetItemAsync<List<CartItem>>("cart");
        }

        private async Task<bool> IsUserAuthenticated()
        {
            return (await authentication.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated;
        }

        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            if (await IsUserAuthenticated())
            {
                await http.PostAsJsonAsync("api/cart/add", cartItem);
            }
            else
            {
                var cart = await GetCartLocalStorage();
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }
                var sameitem = cart
                    .Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
                if (sameitem == null)
                {
                    cart.Add(cartItem);
                }
                else
                {
                    sameitem.Quantity += cartItem.Quantity;
                }

                await localStorage.SetItemAsync("cart", cart);
            }

            await GetCartItensCount();
        }

        public async Task<List<CartProductDTO>> GetCartProducts()
        {
            if (await IsUserAuthenticated())
            {
                var response = await http.GetFromJsonAsync<ServiceResponse<List<CartProductDTO>>>("api/Cart");
                return response.Data;
            }
            else
            {
                var cartitem = await GetCartLocalStorage();
                if (cartitem == null)
                    return new List<CartProductDTO>();
                var response = await http.PostAsJsonAsync("api/Cart/products", cartitem);
                var cartproducts =
                    await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductDTO>>>();
                return cartproducts.Data;
            }
        }

        public async Task RemoveProductCart(int productId, int productTypeId)
        {
            if(await IsUserAuthenticated())
            {
                await http.DeleteAsync($"api/cart/{productId}/{productTypeId}");
            }
            else
            {
                var cart = await GetCartLocalStorage();
                if (cart == null)
                {
                    return;
                }

                var cartitem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);
                if (cartitem != null)
                {
                    cart.Remove(cartitem);
                    await localStorage.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task UpdateQuantity(CartProductDTO product)
        {
            if (await IsUserAuthenticated())
            {
                var request = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductTypeId = product.ProductTypeId,
                    Quantity = product.Quantity
                };
                await http.PutAsJsonAsync("api/cart/update-quantity", request);
            }
            else
            {
                var cart = await GetCartLocalStorage();
                if (cart == null)
                {
                    return;
                }

                var cartitem = cart.Find(x => x.ProductId == product.ProductId && x.ProductTypeId == product.ProductTypeId);
                if (cartitem != null)
                {
                    cartitem.Quantity = product.Quantity;
                    await localStorage.SetItemAsync("cart", cart);
                }
            }
        }

        public async Task StoreCartItems(bool emptylocalcart = false)
        {
            var localcart = await GetCartLocalStorage();
            if (localcart == null)
            {
                return;
            }

            await http.PostAsJsonAsync("api/cart", localcart);
            if (emptylocalcart)
            {
                await localStorage.RemoveItemAsync("cart");
            }
        }

        public async Task GetCartItensCount()
        {
            if (await IsUserAuthenticated())
            {
                var result = await http.GetFromJsonAsync<ServiceResponse<int>>("api/cart/count");
                var count = result.Data;

                await localStorage.SetItemAsync<int>("count", count);
            }
            else
            {
                var cart = await localStorage.GetItemAsync<List<CartItem>>("cart");
                await localStorage.SetItemAsync("count", cart != null ? cart.Count : 0);
            }

            OnChange.Invoke();
        }
    }
}
