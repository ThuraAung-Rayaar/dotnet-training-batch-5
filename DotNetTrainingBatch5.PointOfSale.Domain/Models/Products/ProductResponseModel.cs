using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Models.Products
{
    public class ProductResponseModel
    {
        

        public string? ProductCode { get; set; } = null!;

        public string? Name { get; set; } = null!;

        public decimal Price { get; set; } = 0;

        public string? ProductCategoryCode { get; set; } = null!;

       


    }


}
