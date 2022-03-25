namespace BlazorECommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext context;

        public ProductService(DataContext context)
        {
            this.context = context;
        }

        public async Task<ServiceResponse<Product>> GetProduct(int Id)
        {
            var response = new ServiceResponse<Product>();
            var product = await context.Products
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
                Data = await context.Products
                .Include(x => x.ProductVariants)
                .ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<ProductsViewModel>> GetProductsPage(int page)
        {
            var pageresults = 4f;
            var number = await context.Products.CountAsync();
            var pagecount = Math.Ceiling(number / pageresults);

            var products = await context.Products
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

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryurl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products
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
            var pageresults = 4f;
            var pagecount = Math.Ceiling((await FindProductsBySearchText(searchtext)).Count / pageresults);

            var products = await context.Products
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
            return await context.Products
                            .Where(
                                p => p.Title.ToLower().Contains(searchtext.ToLower())
                                || p.Description.ToLower().Contains(searchtext.ToLower()))
                            .Include(p => p.ProductVariants)
                            .ToListAsync();
        }

        public async Task<ServiceResponse<List<ProductType>>> GetProductsTypeAsync()
        {
            var producttype = await context.ProductType.ToListAsync();
            return new ServiceResponse<List<ProductType>>
            {
                Data = producttype
            };
        }

        public async Task<ServiceResponse<Product>> AddProduct(DTO_Product_ProductType dto)
        {
            context.Products.Add(dto.Product);
            await context.SaveChangesAsync();
            await AddProductVariant(dto);
            return new ServiceResponse<Product>
            {
                Data = dto.Product,
                Message = "Product added with success!!"
            };
        }

        public async Task AddProductVariant(DTO_Product_ProductType dto)
        {
            foreach (var item in dto.ProductType)
            {
                ProductVariant productVariant = new ProductVariant();
                productVariant.ProductId = dto.Product.Id;
                productVariant.ProductTypeId = item.Id;
                context.ProductVariant.Add(productVariant);
                await context.SaveChangesAsync();
            }
        }

        public async Task<ServiceResponse<List<Product>>> GetFeaturedProducts()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await context.Products
                .Where(x => x.Featured)
                .Include(x => x.ProductVariants)
                .ToListAsync()
            };
            return response;
        }
    }
}
