namespace BlazorECommerce.Server.Services.ProductService
{
    public interface IProductService
    {
        Task<ServiceResponse<List<Product>>> GetProducts();
        Task<ServiceResponse<Product>> GetProduct(int Id);
        Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryurl);
        Task<ServiceResponse<ProductsViewModel>> SearchProduct(string searchtext, int page);
        Task<ServiceResponse<List<string>>> GetProductsSearchSuggestions(string searchtext);
        Task<ServiceResponse<List<ProductType>>> GetProductsTypeAsync();
        Task<ServiceResponse<Product>> AddProduct(DTO_Product_ProductType dto);
        Task AddProductVariant(DTO_Product_ProductType dto);
    }
}
