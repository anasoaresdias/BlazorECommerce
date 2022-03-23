using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorECommerce.Shared.ViewModels_dto
{
    public class DTO_Product_ProductType
    {
        public Product Product { get; set; }
        public List<ProductType> ProductType { get; set; }
        public List<ProductVariant> ProductVariant { get; set; }
    }
}
