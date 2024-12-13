using DotNetTrainingBatch5.PointOfSale.Domain.Features.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DotNetTrainingBatch5.PointOfSale.Api.Endpoints.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly SaleService _Service;
        
        public SaleController(SaleService service)
        {
            _Service = service;
            
      
        }

        [HttpGet("/Sale/")]
        public async Task<IActionResult> GetAllSaleAsync()
        {
            try
            {
                var result = await _Service.GetSaleAsync();

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


        [HttpGet("/Sale/voucherNum")]
        public async Task<IActionResult> GetLastSaleVoucher()
        {
            try
            {
                var result = await _Service.GetLatestSaleCodeAsync();

                return Ok($"Last Voucher Number : \"{result}\"");
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
