using DotNetTrainingBatch5.PointOfSale.Domain.Features.Sales;
using DotNetTrainingBatch5.PointOfSale.Domain.Models.SaleDetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DotNetTrainingBatch5.PointOfSale.Api.Endpoints.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleDetailController : ControllerBase
    {

        private readonly SaleDetailServices _Service;
        public SaleDetailController( SaleDetailServices service2)
        {
          
            _Service = service2;

        }

        [HttpGet("/Detail/Voucher-Number={voucher}")]
        public async Task<IActionResult> GetSaleDetailByVoucherAsync([FromQuery] string voucher)
        {
            try
            {
                voucher= voucher.ToUpper();
                var result = await _Service.GetSaleByCodeAsync(voucher);

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

        [HttpGet("/Detail/Product-code={Product}")]
        public async Task<IActionResult> GetSaleDetailByProductAsync(string Product)
        {
            try
            {
                Product= Product.ToUpper();
                var result = await _Service.GetSaleByCodeAsync(Product);

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

        [HttpPost("/Detail/")]
        public async Task<IActionResult> CreateSaleDetail( DetailRequestModel detail)
        {
            try
            {
               if(!detail.ProductCode.IsNullOrEmpty()) detail.ProductCode = detail.ProductCode.ToUpper();
                var result = await _Service.CreateSaleDetail(detail);

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

    }
}
