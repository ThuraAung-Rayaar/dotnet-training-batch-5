using DotNetTrainingBatch5.PointOfSale.DataBase.Models;
using DotNetTrainingBatch5.PointOfSale.Domain.Features.Products;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;


namespace DotNetTrainingBatch5.PointOfSale.Api.Endpoints.Products;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
   /* private readonly ProductService _service;

    public ProductController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetProduct()
    {
        try
        {
            var result = await _service.GetProductsAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var result = await _service.GetProductAsync(id);
            if (result is null)
            {
                return NotFound("No data found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductReqModel reqModel)
    {
        try
        {
            var result = await _service.CreateProductAsync(reqModel.ProductCode, reqModel.ProductName, reqModel.Price, reqModel.InstockQuantity);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { error = ex.Message });
        }
    }



    [HttpPatch]
    public async Task<IActionResult> EditProduct([FromBody] ProductReqModel reqModel)
    {
        try
        {
            var result = await _service.UpdateProductAsync(reqModel.ProductId, reqModel.ProductCode, reqModel.ProductName, reqModel.Price, reqModel.InstockQuantity);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var result = await _service.DeleteProductAsync(id);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }*/


    private readonly ProductService _Service;
    public ProductController(ProductService service)
    {
        _Service = service;
    }


    [HttpGet("/Product/")]
    public async Task<IActionResult> GetProduct()
    {
        try
        {
            var result = await _Service.GetProductsAsync();

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

    [HttpGet("/Product/{code}")]
    public async Task<IActionResult> GetProduct( string code)
    {
        try
        {
            code = code.ToUpper();
            var result = await _Service.GetOneProductAsync(code);
            if (result is null)
            {
                return NotFound("No data found");
            }
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("/Product/")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductReqModel reqModel)
    {
        try
        {
            var result = await _Service.CreateProductAsync(reqModel);
            return Ok(result);
        }
        catch (Exception ex)
        {

            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPatch("/Product/{code}")]
    public async Task<IActionResult> EditProduct( string code,[FromBody] ProductReqModel reqModel)
    {
        try
        {
            code = code.ToUpper();
            var result = await _Service.UpdateProductAsync(code,reqModel);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpDelete("/Product/{code}")]
    public async Task<IActionResult> DeleteProduct( string code)
    {
        try
        {
            code = code.ToUpper();
            var result = await _Service.DeleteProductAsync(code);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }



}
