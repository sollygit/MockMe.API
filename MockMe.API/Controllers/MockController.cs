using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockMe.API.Services;
using MockMe.Model;
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

        [HttpPost("Submission")]
        public async Task<IActionResult> Submission([FromBody] MockRequest request)
        {
            if (request == null)
            {
                return BadRequest(ModelState);
            }

            var data = await _mockService.GetDataAsync(request.SubmissionId);
            return new OkObjectResult(data);
        }

        [HttpGet("RunExe/{filename}")]
        public async Task<IActionResult> RunExe(string filename)
        {
            var result = await _mockService.RunExeAsync(filename);
            return Ok(result);
        }
    }
}
