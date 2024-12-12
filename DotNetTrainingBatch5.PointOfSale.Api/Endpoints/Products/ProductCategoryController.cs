using DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTrainingBatch5.PointOfSale.Api.Endpoints.Products;

[Route("api/[controller]")]
[ApiController]
public class ProductCategoryController : ControllerBase
{
    private readonly ProductCategoryService _services;

    public ProductCategoryController(ProductCategoryService services)
    {
        _services = services;
    }


    [HttpGet("/Category/")]
    public async Task<IActionResult> GetAllCategoryAsync()
    {

        try
        {

            var ItemList = await _services.GetCategoriesAsync();
            return Ok(ItemList);

        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }


    }

    [HttpGet("/Category/{code}")]
    public async Task<IActionResult> GetCategoryByCodeAsync(string code)
    {
        try
        {
            code = code.ToUpper(); 
            var result = await _services.GetCategoryByCodeAsync(code);
            
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("/Category")]
    public async Task<IActionResult> CreateCategoryAsync([FromBody] CategoryRequestModel model)
    {
        try
        {
            var result = await _services.CreateCategoryAsync(model);
            
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("/Category/{code}")]
    public async Task<IActionResult> UpdateCategoryAsync(string code, [FromBody] CategoryRequestModel model)
    {
        try
        {
            code = code.ToUpper(); 
            var result = await _services.UpdateCategoryAsync(code, model);
           
            return Ok(result);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("/Category/{code}")]
    public async Task<IActionResult> DeleteCategoryAsync(string code)
    {
        try
        {
            code = code.ToUpper(); 
            var result = await _services.DeleteCategoryAsync(code);
            
            return Ok(new { message = "Delete successful" });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

}
