namespace BlazorECommerce.Server.Services.OrderService
{
    public class OrderService : IOrderService
    {
        private readonly IHttpContextAccessor http;
        private readonly DataContext context;
        private readonly ICartService cartService;

        public OrderService(IHttpContextAccessor http, DataContext context, ICartService cartService)
        {
            this.http = http;
            this.context = context;
            this.cartService = cartService;
        }

        private int GetUserId() => int.Parse(http.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<bool>> PlaceOrder()
        {
            var products = (await cartService.GetDbCart()).Data;
            decimal totalprice = 0;
            products.ForEach(p => totalprice += p.Price * p.Quantity);

            var orderitem = new List<OrderItem>();
            products.ForEach(p => orderitem.Add(new OrderItem
            {
                ProductId = p.ProductId,
                ProductTypeId = p.ProductTypeId,
                Quantity = p.Quantity,
                TotalPrice = p.Price * p.Quantity
            }));

            var order = new Order
            {
                UserId = GetUserId(),
                OrderDate = DateTime.Now,
                TotalPrice = totalprice,
                OrderItems = orderitem
            };

            context.Orders.Add(order);

            context.CartItems.RemoveRange(context.CartItems
                .Where(x => x.UserId == GetUserId()));

            await context.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }
    }
}
