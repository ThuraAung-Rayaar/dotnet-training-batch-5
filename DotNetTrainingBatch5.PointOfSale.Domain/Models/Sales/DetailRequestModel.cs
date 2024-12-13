using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Models.SaleDetail
{
    public class DetailRequestModel
    {

       

        public string? VoucherNo { get; set; } = "";

        public string? ProductCode { get; set; } = "";

        public int Quantity { get; set; } = 0;

       // public decimal Price { get; set; } = 0M;

    }
}
