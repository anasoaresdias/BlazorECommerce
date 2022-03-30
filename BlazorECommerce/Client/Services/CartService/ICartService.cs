namespace BlazorECommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnChange;
        Task AddToCart(CartItem cartItem);
        Task<List<CartProductDTO>> GetCartProducts();
        Task RemoveProductCart(int productId, int productTypeId);
        Task UpdateQuantity(CartProductDTO product);
        Task StoreCartItems(bool emptylocalcart);
        Task GetCartItensCount();
    }
}
