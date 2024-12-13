using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Models.Sales
{
    public class DetailResponseModel
    {
        public string? VoucherNo { get; set; } = null;

        public string? ProductCode { get; set; } = null;

        public int Quantity { get; set; } = 0;

        public decimal Price { get; set; } = 0M;

    }
}
