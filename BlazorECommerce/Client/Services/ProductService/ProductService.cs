
using BlazorECommerce.Shared;
using System.Net.Http.Json;

namespace BlazorECommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient http;

        public event Action ProductsChanged;

        public List<Product> Products { get; set; } = new List<Product>();
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string LastSearchedText { get; set; } = string.Empty;
        public string Message { get; set; } = "Loading Products....";

        public ProductService(HttpClient http)
        {
            this.http = http;
        }

        public async Task GetProducts(string? categoryurl = null)
        {
            var result = categoryurl == null ?
                await http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product")
                : await http.GetFromJsonAsync<ServiceResponse<List<Product>>>($"api/product/category/{categoryurl}");
            if (result != null && result.Data != null)
                Products = result.Data;
            CurrentPage = 1;
            PageCount = 0;
            if (Products.Count == 0)
                Message = "No products found.";
            ProductsChanged.Invoke();
        }

        public async Task GetListProducts()
        {

            var result = await http.GetFromJsonAsync<ServiceResponse<List<Product>>>("api/product");
            if (result != null && result.Data != null)
                Products = result.Data;
            CurrentPage = 1;
            PageCount = 0;
            if (Products.Count == 0)
                Message = "No products found.";
            ProductsChanged.Invoke();
        }

        public async Task GetProductsPage(int page)
        {
            var result = await http.GetFromJsonAsync<ServiceResponse<ProductsViewModel>>($"api/product/page/{page}");
            Products = result.Data.Products;
            CurrentPage = result.Data.CurrentPage;
            PageCount = result.Data.Pages;
            if (Products.Count == 0)
            {
                Message = "No products found.";
            }
            ProductsChanged.Invoke();
        }

        public async Task<ServiceResponse<Product>> GetProduct(int Id)
        {
            var responde = await http.GetFromJsonAsync<ServiceResponse<Product>>($"api/product/{Id}");
            return responde;
        }

        public async Task SearchProducts(string searchtext, int page)
        {
            LastSearchedText = searchtext;
            var result = await http.GetFromJsonAsync<ServiceResponse<ProductsViewModel>>($"api/product/search/{searchtext}/{page}");
            Products = result.Data.Products;
            CurrentPage = result.Data.CurrentPage;
            PageCount = result.Data.Pages;
            if (Products.Count == 0)
            {
                Message = "No products found.";
            }
            ProductsChanged.Invoke();
        }

        public async Task<List<string>> GetProductsBySearchSuggestions(string searchtext)
        {
            var results = await http.GetFromJsonAsync<ServiceResponse<List<string>>>($"api/product/SearchSuggestions/{searchtext}");
            return results.Data;
        }

        public async Task AddProduct(DTO_Product_ProductType dto)
        {
            await http.PostAsJsonAsync("api/product", dto);
        }

        public async Task<ServiceResponse<List<ProductType>>> GetProductType()
        {
            var responde = await http.GetFromJsonAsync<ServiceResponse<List<ProductType>>>("api/product/ProductType");
            ProductTypes = responde.Data;
            return responde;
        }
    }
}
