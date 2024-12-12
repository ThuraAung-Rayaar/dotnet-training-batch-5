﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotNetTrainingBatch5.PointOfSale.DataBase.Models;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Product;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;

public class ProductService
{
    /*  private readonly AppDbContext _db;


      public ProductService(AppDbContext context)
      {
          _db = context;
      }

      public async Task<Result<ResultProductResponseModel>> GetProductAsync(int id)
      {
          Result<ResultProductResponseModel> model = new Result<ResultProductResponseModel>();


          var product = await _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == id);


          if (product is null)
          {
              model = Result<ResultProductResponseModel>.SystemError("Product not found.");
              goto Result;
          }


          var responseModel = new ResultProductResponseModel
          {
              Product = product
          };


          model = Result<ResultProductResponseModel>.Success(responseModel, "Product retrieved successfully.");

      Result:
          return model;

      }

      public async Task<Result<List<ResultProductResponseModel>>> GetProductsAsync()
      {

          Result<List<ResultProductResponseModel>> model = new Result<List<ResultProductResponseModel>>();


          var products = await _db.TblProducts.AsNoTracking().ToListAsync();


          if (products is null)
          {
              model = Result<List<ResultProductResponseModel>>.SystemError("No products found.");
              return model;
          }


          var responseModels = products.Select(product => new ResultProductResponseModel
          {
              Product = product
          }).ToList();


          model = Result<List<ResultProductResponseModel>>.Success(responseModels, "Products retrieved successfully.");

          return model;
      }


      public async Task<Result<ResultProductResponseModel>> UpdateProductAsync(int id, string productCode, string productName, decimal price ,int instockQuantity)
      {

          Result<ResultProductResponseModel> model = new Result<ResultProductResponseModel>();


          var product = await _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == id);


          if (product is null)
          {

              model = Result<ResultProductResponseModel>.SystemError("Product not found.");
              return model;
          }


          var newProduct = new TblProduct
          {
              ProductCode = productCode,
              ProductName = productName,
              Price = price,
              InstockQuantity = instockQuantity,

          };

          _db.Entry(product).State = EntityState.Modified;
          await _db.SaveChangesAsync();


          var responseModel = new ResultProductResponseModel
          {
              Product = newProduct
          };


          model = Result<ResultProductResponseModel>.Success(responseModel, "Product updated successfully.");
          return model;
      }


      public async Task<Result<ResultProductResponseModel>> CreateProductAsync(string productCode, string productName, decimal price, int InstockQuantity)
      {
          Result<ResultProductResponseModel> model = new Result<ResultProductResponseModel>();


          var existingProductCode = _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(x => x.ProductCode == x.ProductCode);

          if(existingProductCode is null)
          {
             model = Result<ResultProductResponseModel>.SystemError("Product with the same code already exists.");
              goto Result;
           }

          if (productCode.Length > 4)
          {
              model = Result<ResultProductResponseModel>.ValidationError("productCode must be 4 character");
              goto Result;
          }


          var newProduct = new TblProduct
          {
              ProductCode = productCode,
              ProductName = productName,
              Price = price,
              InstockQuantity = InstockQuantity,

          };

          await _db.TblProducts.AddAsync(newProduct);
          //_db.Entry(newProduct).State = EntityState.Modified;
          await _db.SaveChangesAsync();

          var item = new ResultProductResponseModel
          {
              Product =  newProduct
          };
          model = Result<ResultProductResponseModel>.Success(item, "Success.");

          Result:
          return model;
      }

      public async Task<Result<ResultProductResponseModel>> DeleteProductAsync(int id)
      {
          Result<ResultProductResponseModel> model = new Result<ResultProductResponseModel>();


          var product = await _db.TblProducts.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == id);


          if (product is null)
          {
              model = Result<ResultProductResponseModel>.SystemError("Product not found.");
              goto Result;
          }


          var responseModel = new ResultProductResponseModel
          {
              Product = product
          };


          model = Result<ResultProductResponseModel>.Success(responseModel, "Product retrieved successfully.");

      Result:
          return model;
      }

  */


    private readonly POSDbContext _dbContext;

    public ProductService(POSDbContext dbContext)
    {
        _dbContext = dbContext;
    }

 


    public async Task<Result<List<ProductResponseModel>>> GetProductsAsync(){

        var ItemList =await _dbContext.Products.Where(x=>!x.DeleteFlag).ToListAsync();

        if (ItemList.Count == 0 || ItemList is null) {

           return  Result<List<ProductResponseModel>>.NotFoundError();

        }

        List<ProductResponseModel> products = new List<ProductResponseModel>();
        foreach (var item in ItemList)
        {
            var productResponse = new ProductResponseModel
            {
                ProductCode = item.ProductCode,
                Name = item.Name,
                Price = item.Price,
                ProductCategoryCode = item.ProductCategoryCode
            };
            products.Add(productResponse);
        }

        return Result < List <ProductResponseModel>>.Success(products);


        }

    public async Task<Result<ProductResponseModel>> GetOneProductAsync(string code)
    {

        var item = await _dbContext.Products.FirstOrDefaultAsync(x => !x.DeleteFlag && x.ProductCode == code);

        if (item is null)
        {

            return Result<ProductResponseModel>.NotFoundError();

        }

        ProductResponseModel product = new ProductResponseModel()

        { ProductCode = item.ProductCode,
                Name = item.Name,
                Price = item.Price,
                ProductCategoryCode = item.ProductCategoryCode
        };
           

        return Result<ProductResponseModel>.Success(product);


    }


    

    public async Task<Result<ProductResponseModel>> CreateProductAsync(ProductReqModel model) {

        #region  Item Code IncreMentation
        var Code = await _dbContext.Products.MaxAsync(x => x.ProductCode);
        // Code.IncrementCode();
        Code = Code.IncrementCode();
        #endregion

        var newProduct = new Product() { 
            ProductCode = Code,
            ProductCategoryCode= model.ProductCategoryCode,
            Price= model.Price,
            Name = model.Name
        
        };
           
        await _dbContext.Products.AddAsync(newProduct);
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0) return Result<ProductResponseModel>.SystemError();

        ProductResponseModel product = new ProductResponseModel()

        {
            ProductCode = newProduct.ProductCode,
            Name = newProduct.Name,
            Price = newProduct.Price,
            ProductCategoryCode = newProduct.ProductCategoryCode
        };

        return Result<ProductResponseModel>.Success(product);

    }



    public async Task<Result<ProductResponseModel>> UpdateProductAsync(string Code, ProductReqModel model) {
        #region  CHECK PRODUCT BY CODE
        var item = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x=>!x.DeleteFlag && x.ProductCode == Code);
        if (item is null)
        {

            return Result<ProductResponseModel>.NotFoundError();

        }
        #endregion
        #region 

        #endregion

        #region Check Update Field

        if (!model.Name.IsNullOrEmpty())
            item.Name = model.Name;

        if (!model.ProductCategoryCode.IsNullOrEmpty())
        item.ProductCategoryCode = model.ProductCategoryCode;

        if (model.Price != 0) item.Price = model.Price;

        #endregion

        #region   SAVE TO DB
        _dbContext.Entry(item).State = EntityState.Modified;
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0) return Result<ProductResponseModel>.SystemError();

        ProductResponseModel product = new ProductResponseModel()

        {
            ProductCode = item.ProductCode,
            Name = item.Name,
            Price = item.Price,
            ProductCategoryCode = item.ProductCategoryCode
        };

        return Result<ProductResponseModel>.Success(product);
        #endregion
    }


    public async Task<Result<string>> DeleteProductAsync(string Code) {
        #region  CHECK PRODUCT BY CODE
        var item = await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => !x.DeleteFlag && x.ProductCode == Code);
        if (item is null)
        {

            return Result<string>.NotFoundError();

        }
        #endregion

        item.DeleteFlag =!item.DeleteFlag;// tem for quick recovery

        #region   SAVE TO DB
        _dbContext.Entry(item).State = EntityState.Modified;
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0) return Result<string>.SystemError();

        ProductResponseModel product = new ProductResponseModel()

        {
            ProductCode = item.ProductCode,
            Name = item.Name,
            Price = item.Price,
            ProductCategoryCode = item.ProductCategoryCode
        };

        return Result<string>.Success("Delete Successful");
        #endregion


    }



}


public static class ServiceHelper {

    public static string IncrementCode(this string Code) {

        
        string charr = Code.Substring(0, 1);
        string numeric = Code.Substring(1);

       
        int num = int.Parse(numeric) + 1;

        string newCode = num.ToString(new string('0',numeric.Length));

       
        return charr + newCode;
    }
}



