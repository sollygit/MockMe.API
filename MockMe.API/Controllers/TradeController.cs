using Microsoft.AspNetCore.Mvc;
using MockMe.API.Services;
using MockMe.API.ViewModels;
using System;
using System.Threading.Tasks;

namespace MockMe.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "trade")]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        readonly ITradeService _tradeService;
        readonly IAssetService _assetService;
        readonly ICountryService _countryService;

        public TradeController(ITradeService tradeService, IAssetService assetService, ICountryService countryService)
        {
            _tradeService = tradeService;
            _assetService = assetService;
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _tradeService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Id can't be empty");

            var item = await _tradeService.GetAsync(id);

            if (item == null)
            {
                return NotFound(id);
            }

            return new ObjectResult(item);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> Generate()
        {
            var item = await _tradeService.GenerateSingleAsync();
            return CreatedAtAction("Generate", new { id = item.Id }, item);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AssetTradeViewModel trade)
        {
            var item = await _tradeService.SaveAsync(trade);
            return CreatedAtAction("Post", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AssetTradeViewModel trade)
        {
            if (await _tradeService.GetAsync(id) == null) return NotFound(id);

            var item = await _tradeService.UpdateAsync(id, trade);
            return new OkObjectResult(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await _tradeService.GetAsync(id) == null) return NotFound(id);

            var item = await _tradeService.DeleteAsync(id);
            return new OkObjectResult(item);
        }

        [HttpGet("Assets")]
        public async Task<IActionResult> Assets()
        {
            var items = await _assetService.GetAssetsAsync();
            return new OkObjectResult(items);
        }

        [HttpGet("Countries")]
        public async Task<IActionResult> Countries()
        {
            var items = await _countryService.GetCountriesAsync();
            return new OkObjectResult(items);
        }
    }
}
