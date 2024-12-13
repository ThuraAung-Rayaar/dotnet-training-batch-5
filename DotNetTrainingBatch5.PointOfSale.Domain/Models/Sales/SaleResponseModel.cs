using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Models.Sales
{
    public class SaleResponseModel
    {

        

        public string VoucherNo { get; set; } = null!;

        public DateTime? SaleDate { get; set; }

        public decimal TotalAmount { get; set; } 



    }
}
