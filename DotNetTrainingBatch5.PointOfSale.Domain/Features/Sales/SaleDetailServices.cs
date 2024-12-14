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

    /*  private readonly AppDbContext _appDb = new AppDbContext();

     public async Task<Result<ResultSaleDetailResModel>?> GetSaleDetailAsync() {

      var itemList = await _appDb.TblSaleDetails.Where(x=>x.DeleteFlag == false).ToListAsync();

          if (itemList.Count == 0 || itemList is null) return Result<ResultSaleDetailResModel>.NotFoundError();

          ResultSaleDetailResModel result = new ResultSaleDetailResModel() { SaleDeails = itemList};
          return Result<ResultSaleDetailResModel>.Success(result);



      }

      public async Task<Result<ResultSaleDetailResModel>?> GetSaleDetailByIdAsync(int id)
      {

          var itemList = await _appDb.TblSaleDetails.Where(x => x.DeleteFlag == false && (x.DetailId == id)).ToListAsync();
          if (itemList.Count == 0 || itemList is null) return Result<ResultSaleDetailResModel>.NotFoundError();

          ResultSaleDetailResModel result = new ResultSaleDetailResModel() { SaleDeails = itemList };
          return Result<ResultSaleDetailResModel>.Success(result);



      }

      public async Task<Result<ResultSaleDetailResModel>?> GetSaleDetailByCodeAsync(string code)
      {

          var itemList = await _appDb.TblSaleDetails.Where(x => x.DeleteFlag == false &&( x.DetailCode == code || x.ProductCode == code || x.SaleCode == code)).ToListAsync();
          if (itemList.Count == 0 || itemList is null) return Result<ResultSaleDetailResModel>.NotFoundError();

          ResultSaleDetailResModel resultList = new ResultSaleDetailResModel() { SaleDeails = itemList };
          return Result<ResultSaleDetailResModel>.Success(resultList);



      }



      //function to be edit for sale AMM
      public async Task<SaleReqModel?> CreateShellSaleAsync() {
          TblSale sale = new TblSale()
          {
              PayAmount = 0,
              SaleDate = DateTime.Now,
              ChangeAmount = 0,
              TotalSale = 0


          };

          await _appDb.TblSales.AddAsync(sale);
          int result = await _appDb.SaveChangesAsync();
          if (result == 0) return null;

         return new SaleReqModel { 
         SaleCode = sale.SaleCode,
         PayAmount = sale.PayAmount
         };
          // possible error for salecode return 

      }
      // need to be in SALE services


      // assume in SALEDETAIL there is more than one product to sell and detail code for each bunch of product sell but the saleCode will be the same for each bunch of product sell (1,1)*,1

      public async Task<Result<ResultSaleDetailResModel>?> CreateSaleDetailFromOneProductAsync(SaleProductReqModel product,SaleDetailReqModel saleReq) // for one product

      {
          // creating temporary shellcode
          //var saleReq = await CreateShellSale();
          if (saleReq is null) return Result<ResultSaleDetailResModel>.InvalidDataError("Error Data Commuting");


          //Adding a  of product to detail 
          TblSaleDetail Detail = new TblSaleDetail()
          {
              ProductCode = product.ProductCode,
              SaleCode = saleReq.SaleCode,
              Total = product.Price * product.SaleQuantity,// price need to be careful for wrong entry
              ProductQuantity = product.SaleQuantity



          };
          await _appDb.TblSaleDetails.AddAsync(Detail);
          int result = await _appDb.SaveChangesAsync();
          if (result == 0) return Result<ResultSaleDetailResModel>.SystemError("Error Saving Detail");

          //using the shell salecode to add data a sale

          var SaleItem =await _appDb.TblSales.AsNoTracking().Where(x=>x.SaleCode==Detail.SaleCode && !x.DeleteFlag).FirstOrDefaultAsync();
          if (SaleItem is null) return Result<ResultSaleDetailResModel>.SystemError("Error retriving Sale");

          SaleItem.TotalSale += Detail.Total;

          _appDb.Entry(SaleItem).State = EntityState.Modified;
         result = await _appDb.SaveChangesAsync ();
          if (result == 0) return Result<ResultSaleDetailResModel>.SystemError("Error adding Saledetail to  Sale");





          ResultSaleDetailResModel resultList = new ResultSaleDetailResModel() { SaleDeails = { Detail } };
          return Result<ResultSaleDetailResModel>.Success(resultList);



      }

      // write by taking normal perimeter

      public async Task<Result<ResultSaleDetailResModel>?> CreateSaleDetailAsync(string saleCode,string productCode,int quantity,decimal Price) 
      {
          var detail = new TblSaleDetail
          {
              SaleCode = saleCode,
              ProductCode = productCode,
              ProductQuantity = quantity,
              Total = Price * quantity
          };

          await _appDb.TblSaleDetails.AddAsync(detail);
          int result = await _appDb.SaveChangesAsync();
          if (result == 0) return Result<ResultSaleDetailResModel>.SystemError("Error Saving Detail");

          var SaleItem = await _appDb.TblSales.AsNoTracking().Where(x => x.SaleCode == saleCode && !x.DeleteFlag).FirstOrDefaultAsync();
          if (SaleItem is null) return Result<ResultSaleDetailResModel>.SystemError("Error retriving Sale");

          SaleItem.TotalSale += detail.Total;

          _appDb.Entry(SaleItem).State = EntityState.Modified;
          result = await _appDb.SaveChangesAsync();
          if (result == 0) return Result<ResultSaleDetailResModel>.SystemError("Error adding Saledetail to  Sale");


          ResultSaleDetailResModel resultList = new ResultSaleDetailResModel() { SaleDeails = { detail } };
          return Result<ResultSaleDetailResModel>.Success(resultList);



      }

      // assume in SALEDETAIL there is more than one product to sell and detail code for each bunch of product sell but the saleCode will be the same for each bunch of product sell (1-1)*-1
      //to update each detail calling detail code is sufficient and after update also make an update in SALE**






      public async Task<Result<ResultSaleDetailResModel>?> UpdateSaleDetailAsync(string detailcode, SaleDetailReqModel model)
      {

          var detailItem = await _appDb.TblSaleDetails.AsNoTracking().Where(x => !x.DeleteFlag && x.DetailCode == detailcode).FirstOrDefaultAsync();
          if (detailItem is null) return Result<ResultSaleDetailResModel>.NotFoundError();

          decimal oldTotal = detailItem.Total;
          decimal pricePerUnit = detailItem.ProductQuantity > 0 ? detailItem.Total / detailItem.ProductQuantity : 0;

          //patch product code
          if (!string.IsNullOrEmpty(model.ProductCode))
          {
              detailItem.ProductCode = model.ProductCode;

              // Fetch price from TblProduct for recalculation
              var product = await _appDb.TblProducts.AsNoTracking().Where(x => x.ProductCode == model.ProductCode).FirstOrDefaultAsync();

              if (product is not null)
              {
                  pricePerUnit = product.Price;
                  detailItem.Total = pricePerUnit * detailItem.ProductQuantity;
              }
          }

          // patch product quatity
          if (model.ProductQuantity > 0)
          {
              detailItem.ProductQuantity = model.ProductQuantity;
              detailItem.Total = pricePerUnit * model.ProductQuantity;
          }

          // patch salecode 
          if (!string.IsNullOrEmpty(model.SaleCode) && model.SaleCode != detailItem.SaleCode)
          {
              var oldSale = await _appDb.TblSales.Where(x => x.SaleCode == detailItem.SaleCode).FirstOrDefaultAsync();

              if (oldSale is not null)
              {
                  oldSale.TotalSale -= oldTotal;
                  _appDb.Entry(oldSale).State = EntityState.Modified;
              }

              var newSale = await _appDb.TblSales.Where(x => x.SaleCode == model.SaleCode).FirstOrDefaultAsync();

              if (newSale is not null)
              {
                  newSale.TotalSale += detailItem.Total;
                  _appDb.Entry(newSale).State = EntityState.Modified;
              }

              detailItem.SaleCode = model.SaleCode;
          }
          else
          {
              // patch for productCode or quantity change
              var sale = await _appDb.TblSales.Where(x => x.SaleCode == detailItem.SaleCode).FirstOrDefaultAsync();

              if (sale is not null)
              {
                  var diff = detailItem.Total - oldTotal;
                  sale.TotalSale += diff;
                  sale.ChangeAmount -=diff;
                  _appDb.Entry(sale).State = EntityState.Modified;
              }
          }


          _appDb.Entry(detailItem).State = EntityState.Modified;
          int result = await _appDb.SaveChangesAsync();

          if (result == 0)
              return Result<ResultSaleDetailResModel>.SystemError("An error occurred while updating sale details.");

          return Result<ResultSaleDetailResModel>.Success(new ResultSaleDetailResModel{ 
                                      SaleDeails = new List<TblSaleDetail> { detailItem }}
                                      );
      }



      //function to change the Tbl_SAle total and the change done by taking the sale code 

      // to change product code just delete and Minus Sale.Total
      //when Deleted Chnage Sale.Total are effected columns


      public async Task<Result<ResultSaleDetailResModel>?> DeleteSaleDetailAsync(string detailCode) { 

          var detailItem =await _appDb.TblSaleDetails.AsNoTracking().Where(x=>x.DetailCode == detailCode&& x.DeleteFlag == false).FirstOrDefaultAsync();

          if (detailItem is null) return Result<ResultSaleDetailResModel>.NotFoundError();

          detailItem.DeleteFlag = true;
          _appDb.Entry(detailItem).State= EntityState.Modified;

          // get sale through detailItem.SaleCode
          var sale = await _appDb.TblSales.AsNoTracking().Where(x => x.SaleCode == detailItem.SaleCode) .FirstOrDefaultAsync();
          if (sale is  null) return Result<ResultSaleDetailResModel>.SystemError("Error retriving Sale");


              sale.TotalSale -= detailItem.Total;
              sale.ChangeAmount  =sale.PayAmount -sale.TotalSale;
              _appDb.Entry(sale).State = EntityState.Modified;

          //subtract from Tblsale.total and change +





          int result = await _appDb.SaveChangesAsync();
          if (result == 0)
              return Result<ResultSaleDetailResModel>.SystemError("An error occurred while deleting  sale details.");

          return Result<ResultSaleDetailResModel>.Success(new ResultSaleDetailResModel
          {
              SaleDeails = new List<TblSaleDetail> { detailItem }
          }
                                      );




      }*/

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

        //Concurrency issue of adding sale and calling it back is occuring 

        #region Check Product to get Price
        var productItem = await _db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductCode == detail.ProductCode);
        if (productItem is null) { return Result<DetailResponseModel>.NotFoundError("Product Code Dont Exist in Products Table"); }
        #endregion


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


