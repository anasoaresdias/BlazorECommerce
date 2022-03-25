namespace BlazorECommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService localStorage;
        private readonly HttpClient http;

        public CartService(ILocalStorageService localStorage, HttpClient http)
        {
            this.localStorage = localStorage;
            this.http = http;
        }
        
        private async Task<List<CartItem>> GetCartLocalStorage()
        {
            return await localStorage.GetItemAsync<List<CartItem>>("cart");
        }

        public event Action OnChange;

        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await GetCartLocalStorage();
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            var sameitem = cart
                .Find(x => x.ProductId == cartItem.ProductId && x.ProductTypeId == cartItem.ProductTypeId);
            if(sameitem == null)
            {
                cart.Add(cartItem);
            }
            else
            {
                sameitem.Quantity += cartItem.Quantity;
            }

            
            await localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await GetCartLocalStorage();
            if (cart == null)
            {
                cart = new List<CartItem>();
            }

            return cart;
        }

        public async Task<List<CartProductDTO>> GetCartProducts()
        {
            var cartitem = await GetCartLocalStorage();
            var response = await http.PostAsJsonAsync("api/Cart/products", cartitem);
            var cartproducts =
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductDTO>>>();
            return cartproducts.Data;
        }

        public async Task RemoveProductCart(int productId, int productTypeId)
        {
            var cart = await GetCartLocalStorage();
            if(cart == null)
            {
                return;
            }

            var cartitem = cart.Find(x=>x.ProductId == productId && x.ProductTypeId == productTypeId);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
                await localStorage.SetItemAsync("cart", cart);
                OnChange.Invoke();
            }

        }

        public async Task UpdateQuantity(CartProductDTO product)
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
}
