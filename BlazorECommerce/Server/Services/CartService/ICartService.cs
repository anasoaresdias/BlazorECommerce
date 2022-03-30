namespace BlazorECommerce.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductDTO>>> GetCartProducts(List<CartItem> carItems);
        Task<ServiceResponse<List<CartProductDTO>>> StoreCartItems(List<CartItem> cartitens);
        Task<ServiceResponse<int>> GetCartItemsCount();
        Task<ServiceResponse<List<CartProductDTO>>> GetDbCart();
        Task<ServiceResponse<bool>> AddToCart(CartItem cart);
        Task<ServiceResponse<bool>> UpdateQuantity(CartItem cart);
        Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId);
    }
}
