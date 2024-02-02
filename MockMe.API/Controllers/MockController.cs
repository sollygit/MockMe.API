using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockMe.API.Services;
using MockMe.Model;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MockMe.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize]
    [Route("api/[controller]")]
    public class MockController : ControllerBase
    {
        readonly IMockService _mockService;

        public MockController(IMockService mockService) =>
            (_mockService) = (mockService);

        [HttpGet("Product")]
        public async Task<IActionResult> Products([FromQuery(Name = "_limit")] int limit)
        {
            var items = await _mockService.GetProductsAsync(limit);
            return new OkObjectResult(items);
        }

        [HttpGet("Country")]
        public async Task<IActionResult> Countries()
        {
            var items = await _mockService.GetCountriesAsync();
            return new OkObjectResult(items);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Product([FromBody] ProductRequest request)
        {
            if (request == null) return BadRequest(ModelState);

            var data = await _mockService.ProductAdd(request);
            return new OkObjectResult(data);
        }

        [HttpGet("RunExe/{filename}")]
        public async Task<IActionResult> RunExe(string filename)
        {
            var result = await _mockService.RunExeAsync(filename);
            return Ok(result);
        }

        [HttpPost("encode")]
        public IActionResult Encode([FromBody] PlainText plainText)
        {
            try
            {
                return Ok(Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText.Text)));
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Encode error: {ex.Message}");
            }
        }

        [HttpPost("decode")]
        public IActionResult Decode([FromBody] PlainText plainText)
        {
            try
            {
                return Ok(Encoding.UTF8.GetString(Convert.FromBase64String(plainText.Text)));
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Decode error: {ex.Message}");
            }
        }
    }
}
