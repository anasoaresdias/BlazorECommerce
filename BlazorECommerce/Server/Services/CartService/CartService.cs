namespace BlazorECommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext context;
        private readonly IHttpContextAccessor httpContext;

        public CartService(DataContext context, IHttpContextAccessor httpContext)
        {
            this.context = context;
            this.httpContext = httpContext;
        }

        private int GetUserId() => int.Parse(httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        public async Task<ServiceResponse<List<CartProductDTO>>> GetCartProducts(List<CartItem> carItems)
        {
            var result = new ServiceResponse<List<CartProductDTO>>
            {
                Data = new List<CartProductDTO>()
            };

            foreach (var item in carItems)
            {
                var product = await context.Products
                    .Where(x => x.Id == item.ProductId)
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

        public async Task<ServiceResponse<List<CartProductDTO>>> StoreCartItems(List<CartItem> cartitens)
        {
            cartitens.ForEach(x => x.UserId = GetUserId());
            context.CartItems.AddRange(cartitens);
            await context.SaveChangesAsync();

            return await GetDbCart();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = (await context.CartItems.Where(x => x.UserId == GetUserId()).ToListAsync()).Count;
            return new ServiceResponse<int> { Data = count };
        }

        public async Task<ServiceResponse<List<CartProductDTO>>> GetDbCart()
        {
            return await GetCartProducts
                (await context.CartItems
                .Where(x => x.UserId == GetUserId())
                .ToListAsync()
                );
        }

        public async Task<ServiceResponse<bool>> AddToCart(CartItem cart)
        {
            cart.UserId = GetUserId();
            var sameitem = await context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.ProductId == cart.ProductId &&
                    x.ProductTypeId == cart.ProductTypeId &&
                    x.UserId == cart.UserId);
            if (sameitem == null)
            {
                context.CartItems.Add(cart);
            }
            else
            {
                sameitem.Quantity += cart.Quantity;
            }

            await context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cart)
        {
            var sameitem = await context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.ProductId == cart.ProductId &&
                    x.ProductTypeId == cart.ProductTypeId &&
                    x.UserId == GetUserId());
            if (sameitem == null)
                return new ServiceResponse<bool> { Data = false, Message = "Cart Item does not exist", Success = false };

            sameitem.Quantity = cart.Quantity;
            await context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var sameitem = await context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.ProductId == productId &&
                    x.ProductTypeId == productTypeId &&
                    x.UserId == GetUserId());
            if (sameitem == null)
                return new ServiceResponse<bool> { Data = false, Message = "Cart Item does not exist", Success = false };
            context.CartItems.Remove(sameitem);
            await context.SaveChangesAsync();
            return new ServiceResponse<bool> { Data = true };
        }
    }
}
