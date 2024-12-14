using DotNetTrainingBatch5.PointOfSale.DataBase.Models;
using DotNetTrainingBatch5.PointOfSale.Domain;
using DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Product;

using DotNetTrainingBatch5.PointOfSale.Domain.Models.SaleDetail;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Sales;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Features.Sales;

public class SaleDetailServices
{

    

    private readonly POSDbContext _db;

    public SaleDetailServices(POSDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<DetailResponseModel>>> GetSaleByCodeAsync(string code)
    {
        
        var sale = await _db.SaleDetails.AsNoTracking().Where(s => s.VoucherNo == code || s.ProductCode == code).ToListAsync();

        if (sale is null)
        {

            return Result<List<DetailResponseModel>>.NotFoundError("Sale Not Found!");

        }

        List<DetailResponseModel> saleRes = new List<DetailResponseModel>();

        //TODO: also grab the associated sale details and products
        foreach (var item in sale)
        {
            DetailResponseModel model = new DetailResponseModel
            {
                ProductCode = item.ProductCode,
                VoucherNo = item.VoucherNo,
                Price = item.Price,
                Quantity = item.Quantity

            };

            saleRes.Add(model);
        }

        return Result<List<DetailResponseModel>>.Success(saleRes);


    }

    public async Task<Result<List<DetailResponseModel>>> GetSaleDetailAsync()
    {
        // Result<SaleResponseModel> response = new Result<SaleResponseModel>();
        var saleDetails = await _db.SaleDetails.AsNoTracking().ToListAsync();
        if (saleDetails is null || !saleDetails.Any())
        {

            return Result<List<DetailResponseModel>>.NotFoundError("Sale Not Found!");

        }

        List<DetailResponseModel> saleRes = new List<DetailResponseModel>();


        foreach (var item in saleDetails)
        {
            DetailResponseModel model = new DetailResponseModel
            {
                ProductCode = item.ProductCode,
                VoucherNo = item.VoucherNo,
                Price = item.Price,
                Quantity = item.Quantity

            };

            saleRes.Add(model);
        }
        return Result<List<DetailResponseModel>>.Success(saleRes);


    }



    public async Task<Result<DetailResponseModel>> CreateSaleDetail(DetailRequestModel detail) {
        //after first product sell and obtaining voucherNo the parameter is to be added

        // using transaction
        using var transaction = await _db.Database.BeginTransactionAsync();//using transaction to solve consurrency issue
        var saleTable = _db.Sales;

        #region Check Product to get Price
        var productItem = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductCode == detail.ProductCode);
        if (productItem is null) { return Result<DetailResponseModel>.NotFoundError("Product Code Dont Exist in Products Table"); }
        #endregion


        #region IF  New Sale Occur 

        string pattern = @"^V\d{3}$";//to follow V000 standard


        if (!detail.VoucherNo.IsNullOrEmpty())
        {
            if (Regex.IsMatch(detail.VoucherNo, pattern))
                return Result<DetailResponseModel>.InvalidDataError("Voucher Not VAlid");

        }



        //if null or empty
        else {

            // calling data table from sale table only one time
            var code = await saleTable.MaxAsync(x => x.VoucherNo);
            //var code = await _db.Sales.AsNoTracking().MaxAsync(x => x.VoucherNo);

            detail.VoucherNo = code.IsNullOrEmpty() ? "V001" : code.IncrementCode();

            Sale ss = new Sale
            {

                VoucherNo = detail.VoucherNo
            ,
                SaleDate = DateTime.Now
            };
            //_db.Entry(code).State = EntityState.Modified;
            _db.Sales.Add(ss);
            if (await _db.SaveChangesAsync() == 0) return Result<DetailResponseModel>.SystemError();
        }
        #endregion

        //Concurrency issue of adding sale and calling it back is occuring ---> use transsaction and no asnotracking to temporary solve the error

       


        #region Adding the saleDetail
        if (detail.Quantity <= 0) return Result<DetailResponseModel>.InvalidDataError("Quantity:  0");
        SaleDetail saleDetail = new SaleDetail() {
            VoucherNo = detail.VoucherNo,
            Price = productItem.Price,
            Quantity = detail.Quantity,
            ProductCode = detail.ProductCode

        };



        await _db.SaleDetails.AddAsync(saleDetail);
        //dont save the sale detail here***
        //int result = await _db.SaveChangesAsync();
        //if(result ==0) return Result<DetailResponseModel>.SystemError("Error saving SaleDetail");
        #endregion


        #region Update Sale

        //use sale table
        //var saleitem = await _db.Sales.AsNoTracking().FirstOrDefaultAsync(x => x.VoucherNo == saleDetail.VoucherNo);
        var saleitem = await saleTable.FirstOrDefaultAsync(x => x.VoucherNo == saleDetail.VoucherNo);
        if (saleitem is null) return Result<DetailResponseModel>.InvalidDataError($"Sale not found for  VoucherNo:{saleDetail.VoucherNo}");

        saleitem.SaleDate = DateTime.Now;
        saleitem.TotalAmount += saleDetail.Quantity * saleDetail.Price; 

        //_db.Entry(saleitem).State = EntityState.Modified;
        
        _db.Update(saleitem);


        #endregion

        int result2 = await _db.SaveChangesAsync(); // saving the changes

        if (result2 == 0) return Result<DetailResponseModel>.SystemError("Error  Updating sale");

        ////now save changes to detail
        //int result = await _db.SaveChangesAsync();
        //if (result == 0) return Result<DetailResponseModel>.SystemError("Error saving SaleDetail");

        await transaction.CommitAsync();

        DetailResponseModel res = new DetailResponseModel()
            {
                VoucherNo = saleDetail.VoucherNo,
                Price = saleDetail.Price,
                Quantity = saleDetail.Quantity,
                ProductCode = saleDetail.ProductCode


            };
            return Result<DetailResponseModel>.Success(res);


      
       

    }

    

}


