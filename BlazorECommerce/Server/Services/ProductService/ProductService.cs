namespace BlazorECommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Product>> GetProduct(int Id)
        {
            var response = new ServiceResponse<Product>();
            var product = await _context.Products
                .Include(x=>x.ProductVariants)
                .ThenInclude(x=>x.ProductType)
                .FirstOrDefaultAsync(p => p.Id == Id);
            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry, but this product does not exist.";
            }
            else
            {
                response.Data = product;
            }

            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                .Include(x => x.ProductVariants)
                .ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryurl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products
                    .Where(p => p.Category.Url.ToLower()
                    .Equals(categoryurl.ToLower()))
                    .Include(x => x.ProductVariants)
                    .ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<List<string>>> GetProductsSearchSuggestions(string searchtext)
        {
            var products = await FindProductsBySearchText(searchtext);
            
            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if(product.Title.Contains(searchtext, StringComparison.OrdinalIgnoreCase))
                    result.Add(product.Title);
                if (product.Description != null)
                {
                    var punctuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();
                    var words = product.Description.Split().Select(w => w.Trim(punctuation));
                    foreach (var word in words)
                    {
                        if(word.Contains(searchtext, StringComparison.OrdinalIgnoreCase) && !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    } 
                }
            }
            return new ServiceResponse<List<string>> { Data = result };
        }

        public async Task<ServiceResponse<ProductsViewModel>> SearchProduct(string searchtext, int page)
        {
            var pageresults = 3f;
            var pagecount = Math.Ceiling((await FindProductsBySearchText(searchtext)).Count / pageresults);

            var products = await _context.Products
                            .Where(
                                p => p.Title.ToLower().Contains(searchtext.ToLower())
                                || p.Description.ToLower().Contains(searchtext.ToLower()))
                            .Include(p => p.ProductVariants)
                            .Skip((page - 1) * (int)pageresults)
                            .Take((int)pageresults)
                            .ToListAsync();

            var response = new ServiceResponse<ProductsViewModel>
            {
                Data = new ProductsViewModel
                {
                    Products = products,
                    CurrentPage = page,
                    Pages = (int)pagecount
                }
            };
            return response;
        }

        private async Task<List<Product>> FindProductsBySearchText(string searchtext)
        {
            return await _context.Products
                            .Where(
                                p => p.Title.ToLower().Contains(searchtext.ToLower())
                                || p.Description.ToLower().Contains(searchtext.ToLower()))
                            .Include(p => p.ProductVariants)
                            .ToListAsync();
        }
    }
}
