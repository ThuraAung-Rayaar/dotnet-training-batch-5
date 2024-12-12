using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNetTrainingBatch5.PointOfSale.DataBase.Models;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Models.Product
{
    public class ProductReqModel
    {


       

        //public string ProductCode { get; set; } = null!;
        
        public string? Name { get; set; } = null;

        public decimal Price { get; set; } = 0M;

        public string? ProductCategoryCode { get; set; } = null;





    }

    public class SaleProductReqModel
    {
       

        public string ProductCode { get; set; } = null!;

        public decimal Price { get; set; } 

        public int SaleQuantity { get; set; }



    }

}
