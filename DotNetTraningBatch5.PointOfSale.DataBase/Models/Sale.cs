using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetTrainingBatch5.PointOfSale.DataBase.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public string VoucherNo { get; set; }// = null!;

    public DateTime? SaleDate { get; set; } = DateTime.MinValue;

    public decimal TotalAmount { get; set; } = 0M;

    [Timestamp]
    public byte[] RowVersion { get; set; }
}
