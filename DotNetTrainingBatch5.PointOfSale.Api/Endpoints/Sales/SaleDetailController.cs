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

        private readonly SaleDetailServices _Service2;
        public SaleDetailController( SaleDetailServices service2)
        {
          
            _Service2 = service2;

        }

        [HttpGet("/Detail/")]
        public async Task<IActionResult> GetAllSaleDetailAsync()
        {
            try
            {
                var result = await _Service2.GetSaleDetailAsync();

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
        public async Task<IActionResult> CreateSaleDetail(DetailRequestModel detail)
        {
            try
            {
               if(!detail.ProductCode.IsNullOrEmpty()) detail.ProductCode = detail.ProductCode.ToUpper();
                var result = await _Service2.CreateSaleDetail(detail);

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
