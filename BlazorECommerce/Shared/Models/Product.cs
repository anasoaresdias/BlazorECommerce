
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorECommerce.Shared
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
        public bool Featured { get; set; } = false;
        public int Stock { get; set; } = 0;
        public List<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();
    }
}
