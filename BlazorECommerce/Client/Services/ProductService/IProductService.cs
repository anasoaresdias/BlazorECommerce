using BlazorECommerce.Shared;

namespace BlazorECommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        event Action ProductsChanged;
        List<Product> Products { get; set; }
        List<ProductType> ProductTypes { get; set; }
        string Message { get;set; }
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        string LastSearchedText { get; set; }
        Task GetProducts(string? categoryurl = null);
        Task<ServiceResponse<Product>> GetProduct(int Id);
        Task SearchProducts(string searchtext, int page);
        Task<List<string>> GetProductsBySearchSuggestions(string searchtext);
        Task AddProduct(DTO_Product_ProductType dto);
        Task<ServiceResponse<List<ProductType>>> GetProductType();

    }
}
