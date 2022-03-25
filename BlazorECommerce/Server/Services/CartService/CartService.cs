namespace BlazorECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext context;

        public CartService(DataContext context)
        {
            this.context = context;
        }
        public async Task<ServiceResponse<List<CartProductDTO>>> GetCartProducts(List<CartItem> carItems)
        {
            var result = new ServiceResponse<List<CartProductDTO>>
            {
                Data = new List<CartProductDTO>()
            };

            foreach (var item in carItems)
            {
                var product = await context.Products
                    .Where(x=>x.Id == item.ProductId)
                    .FirstOrDefaultAsync();
                if (product == null)
                    continue;

                var productvariant = await context.ProductVariant
                    .Where(x => x.ProductId == item.ProductId && x.ProductTypeId == item.ProductTypeId)
                    .Include(x => x.ProductType)
                    .FirstOrDefaultAsync();

                if (productvariant == null)
                    continue;

                var cartproduct = new CartProductDTO
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageURL = product.ImageUrl,
                    Price = productvariant.Price,
                    ProductType = productvariant.ProductType.Name,
                    ProductTypeId = productvariant.ProductTypeId,
                    Quantity = item.Quantity
                };

                result.Data.Add(cartproduct);
            }
            return result;
        }
    }
}
