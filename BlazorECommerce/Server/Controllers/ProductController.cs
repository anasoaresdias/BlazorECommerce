using Microsoft.AspNetCore.Mvc;

namespace BlazorECommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService productService;
        public ProductController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProducts()
        {
            var response = await productService.GetProducts();
            return Ok(response);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int Id)
        {
            var response = await productService.GetProduct(Id);
            return Ok(response);
        }

        [HttpGet]
        [Route("Category/{categoryurl}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProductsByCategory(string categoryurl)
        {
            var response = await productService.GetProductsByCategory(categoryurl);
            return Ok(response);
        }

        [HttpGet("search/{searchtext}/{page}")]
        public async Task<ActionResult<ServiceResponse<ProductsViewModel>>> SearchProduct(string searchtext, int page = 1)
        {
            var response = await productService.SearchProduct(searchtext, page);
            return Ok(response);
        }

        [HttpGet("SearchSuggestions/{searchtext}")]
        public async Task<ActionResult<ServiceResponse<List<string>>>> GetProductSearchSuggestions(string searchtext)
        {
            var response = await productService.GetProductsSearchSuggestions(searchtext);
            return Ok(response);
        }

        [HttpGet("ProductType")]
        public async Task<ActionResult<ServiceResponse<List<ProductType>>>> GetProductTypeAsync()
        {
            var results = await productService.GetProductsTypeAsync();
            return Ok(results);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Product>>> AddProduct(DTO_Product_ProductType dto)
        {
            var response = await productService.AddProduct(dto);
            return Ok(response);
        }
    }

}

