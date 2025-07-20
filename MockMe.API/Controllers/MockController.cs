using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockMe.API.Services;
using MockMe.Common;
using MockMe.Model;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MockMe.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
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

        [Authorize]
        [HttpPost("[action]")]
        public async Task<IActionResult> Product([FromBody] ProductRequest request)
        {
            if (request == null) return BadRequest(ModelState);

            var data = await _mockService.ProductAdd(request);
            return new OkObjectResult(data);
        }

        [Authorize]
        [HttpGet("Country")]
        public async Task<IActionResult> Countries()
        {
            var items = await _mockService.GetCountriesAsync();
            return new OkObjectResult(items);
        }

        [HttpPost("Encode")]
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

        [HttpPost("Decode")]
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

        [HttpGet(nameof(Reports))]
        public async Task<IActionResult> Reports()
        {
            var path = @$"{Directory.GetCurrentDirectory()}\Shared\reports.json";
            var json = await System.IO.File.ReadAllTextAsync(path);
            var items = Deserializer.FromJsonDictionary<Report>(json).OrderBy(o => o.Key);
            return new OkObjectResult(items);
        }

        [Authorize]
        [HttpGet(nameof(Technologies))]
        public async Task<IActionResult> Technologies()
        {
            var path = @$"{Directory.GetCurrentDirectory()}\Shared\technologies.json";
            var json = await System.IO.File.ReadAllTextAsync(path);
            var items = Deserializer.FromJsonList<Technology>(json).OrderBy(o => o.TechnologyId);
            return new OkObjectResult(items);
        }
    }
}
