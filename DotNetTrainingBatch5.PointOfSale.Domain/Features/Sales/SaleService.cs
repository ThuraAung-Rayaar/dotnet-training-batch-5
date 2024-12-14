using DotNetTrainingBatch5.PointOfSale.DataBase.Models;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Sales;
using DotNetTrainingBatch5.PointOfSale.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;
using Microsoft.IdentityModel.Tokens;


namespace DotNetTrainingBatch5.PointOfSale.Domain.Features.Sales;
public class SaleService
{
   

    private readonly POSDbContext _db;

    public SaleService(POSDbContext db)
    {
        _db = db;
    }

    public async Task<Result<SaleResponseModel>> GetSaleByCodeAsync(string voucher)
    {
       // Result<SaleResponseModel> response = new Result<SaleResponseModel>();
        var sale = await _db.Sales.AsNoTracking().Where(s => s.VoucherNo == voucher).FirstOrDefaultAsync();

        if (sale is null)
        {

            return Result<SaleResponseModel>.NotFoundError("Sale Not Found!");
            
        }

        Result<SaleResponseModel> response = new Result<SaleResponseModel>();

        //TODO: also grab the associated sale details and products
        SaleResponseModel model = new SaleResponseModel { 
        VoucherNo = voucher,
        SaleDate = sale.SaleDate,
        TotalAmount = sale.TotalAmount
        
        };

        return Result<SaleResponseModel>.Success(model);

    
    }

    public async Task<Result<List<SaleResponseModel>>> GetSaleAsync()
    {
        // Result<SaleResponseModel> response = new Result<SaleResponseModel>();
        var sales = await _db.Sales.AsNoTracking().ToListAsync();
        if (sales is null || !sales.Any())
        {

            return Result<List<SaleResponseModel>>.NotFoundError("Sale Not Found!");

        }

          List < SaleResponseModel > saleRes = new List<SaleResponseModel>();

        
        foreach (var item in sales)
        {
            SaleResponseModel model = new SaleResponseModel
            {
                VoucherNo = item.VoucherNo,
                SaleDate = item.SaleDate,
                TotalAmount = item.TotalAmount

            };

            saleRes.Add(model);
        }
        return Result<List<SaleResponseModel>>.Success(saleRes);


    }

    public async Task<string> GetLatestSaleCodeAsync()
    {

        var code = await _db.Sales.MaxAsync(x => x.VoucherNo);
        if (code.IsNullOrEmpty()) return "No item Added yet";
        return code;



    }

}
