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
    /* private readonly AppDbContext _db;

     public SaleService()
     {
         _db = new AppDbContext();
     }

     public async Task<Result<SaleResModel>> GetSaleByCode(string saleCode)
     {
         Result<SaleResModel> response = new Result<SaleResModel>();
         var sale = await _db.TblSales.AsNoTracking().Where(s => s.DeleteFlag == false && s.SaleCode == saleCode).FirstOrDefaultAsync();
         if (sale is null)
         {
             response = Result<SaleResModel>.NotFoundError("Sale Not Found!");
             goto Result;
         }

         //TODO: also grab the associated sale details and products
         SaleResModel model = new SaleResModel { Sale = sale };
         response = Result<SaleResModel>.Success(model, "Sale Found!");

     Result:
         return response;
     }

     //public async Task<Result<List<SaleResModel>>> GetSalesByMonth

     public async Task<Result<SaleResModel>> CreateSale()
     {
         Result<SaleResModel> response = new Result<SaleResModel>();
         TblSale? newSale = default;
         await _db.AddAsync(newSale);
         await _db.SaveChangesAsync();

         SaleResModel saleResModel = new SaleResModel
         {
             Sale = newSale
         };

         response = Result<SaleResModel>.Success(saleResModel, "New Skeleton Sale!");

         return response;
     }

     public async Task<Result<SaleResModel>> RecalculateTotal(string saleCode)
     {
         Result<SaleResModel> response = new Result<SaleResModel>();

         var sale = await _db.TblSales.AsNoTracking().Where(s => s.DeleteFlag == false && s.SaleCode == saleCode).FirstOrDefaultAsync();
         if (sale is null)
         {
             response = Result<SaleResModel>.NotFoundError("Sale Not Found!");
             goto Result;
         }

         var list = await _db.TblSaleDetails
             .AsNoTracking()
             .Where(sd => sd.DeleteFlag == false && sd.SaleCode == saleCode)
             .ToListAsync();
         if (sale is null)
         {
             response = Result<SaleResModel>.NotFoundError("Sale Details are empty!");
             goto Result;
         }

         foreach (var item in list)
         {
             sale.TotalSale += item.Total;
         }

         _db.Entry(sale).State = EntityState.Modified;
         int result = await _db.SaveChangesAsync();

         if (result == 0)
         {
             response = Result<SaleResModel>.Error("Something went wrong, when recalculating total!");
             goto Result;
         }

         SaleResModel saleResModel = new SaleResModel
         {
             Sale = sale,
         };
         response = Result<SaleResModel>.Success(saleResModel, "Sale Recalculated!");

     Result:
         return response;
     }

     public async Task<Result<SaleResModel>> DeleteSale(string saleCode)
     {
         Result<SaleResModel> response = new Result<SaleResModel>();

         var sale = await _db.TblSales.AsNoTracking().Where(s => s.DeleteFlag == false && s.SaleCode == saleCode).FirstOrDefaultAsync();
         if (sale is null)
         {
             response = Result<SaleResModel>.NotFoundError("Sale Not Found!");
             goto Result;
         }

         sale.DeleteFlag = true;
         _db.Entry(sale).State = EntityState.Modified;
         await _db.SaveChangesAsync();

         SaleResModel saleResModel = new SaleResModel { Sale = sale, };
         response = Result<SaleResModel>.Success(saleResModel, "Sale Deleted Successfully");

     Result:
         return response;
     }*/

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
