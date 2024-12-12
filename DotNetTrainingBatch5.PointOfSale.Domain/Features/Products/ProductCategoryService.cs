using DotNetTrainingBatch5.PointOfSale.DataBase.Models;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;

public class ProductCategoryService
{

    private readonly POSDbContext _dbContext;

    public ProductCategoryService(POSDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<Result<List<CategoryResponseModel>>> GetCategoriesAsync()
    {
        var ItemList = _dbContext.ProductCategories.Where(x => !x.DeleteFlag).ToList();
        if (ItemList.Count == 0 || ItemList is null)
        {

            return Result<List<CategoryResponseModel>>.NotFoundError();

        }

        List<CategoryResponseModel> response = new List<CategoryResponseModel>();
        foreach (var item in ItemList)
        {
            response.Add(new CategoryResponseModel() { 
            ProductCategoryCode = item.ProductCategoryCode,
            Name = item.Name
            
            });
        }

        return Result<List<CategoryResponseModel>>.Success(response);

    }
    public async Task<Result<CategoryResponseModel>> GetCategoryByCodeAsync(string code)
    {
        var item = await _dbContext.ProductCategories.FirstOrDefaultAsync(x => !x.DeleteFlag && x.ProductCategoryCode == code);

        if (item is null)
        {
            return Result<CategoryResponseModel>.NotFoundError();
        }

        var category = new CategoryResponseModel
        {
            ProductCategoryCode = item.ProductCategoryCode,
            Name = item.Name
        };

        return Result<CategoryResponseModel>.Success(category);
    }

    public async Task<Result<CategoryResponseModel>> CreateCategoryAsync(CategoryRequestModel model)
    {
       
        

        var newCategory = new ProductCategory 
        { ProductCategoryCode = model.ProductCategoryCode,
            Name = model.Name }; 

        await _dbContext.ProductCategories.AddAsync(newCategory);
        int result = await _dbContext.SaveChangesAsync();
        
        if (result == 0) return Result<CategoryResponseModel>.SystemError(); 

        var category = new CategoryResponseModel { ProductCategoryCode = newCategory.ProductCategoryCode, Name = newCategory.Name }; 

        return Result<CategoryResponseModel>.Success(category); 
    }

    public async Task<Result<CategoryResponseModel>> UpdateCategoryAsync(string code, CategoryRequestModel model)
    {
        var item = await _dbContext.ProductCategories.AsNoTracking().FirstOrDefaultAsync(x => !x.DeleteFlag && x.ProductCategoryCode == code);
        if (item is null)
        {
            return Result<CategoryResponseModel>.NotFoundError();
        }

        if (!string.IsNullOrEmpty(model.Name))
            item.Name = model.Name;

        _dbContext.Entry(item).State = EntityState.Modified;
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0) return Result<CategoryResponseModel>.SystemError();

        var category = new CategoryResponseModel
        {
            ProductCategoryCode = item.ProductCategoryCode,
            Name = item.Name
        };

        return Result<CategoryResponseModel>.Success(category);
    }

    public async Task<Result<string>> DeleteCategoryAsync(string code)
    {
        var item = await _dbContext.ProductCategories
            .FirstOrDefaultAsync(x => !x.DeleteFlag && x.ProductCategoryCode == code);

        if (item is null)
        {
            return Result<string>.NotFoundError();
        }

        item.DeleteFlag = true;
        _dbContext.Entry(item).State = EntityState.Modified;

        int result = await _dbContext.SaveChangesAsync();

        if (result == 0)
        {
            return Result<string>.SystemError();
        }

        return Result<string>.Success("Delete Successful");
    }

}
