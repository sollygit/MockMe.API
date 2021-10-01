using Microsoft.AspNetCore.Mvc;
using MockMe.API.Services;
using MockMe.API.ViewModels;
using System;
using System.Threading.Tasks;

namespace MockMe.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/[controller]")]
    public class TradeController : ControllerBase
    {
        private readonly ITradeService tradeService;
        private readonly IAssetService assetService;

        public TradeController(ITradeService tradeService, IAssetService assetService)
        {
            this.tradeService = tradeService;
            this.assetService = assetService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await tradeService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty) return BadRequest("Id can't be empty");

            var item = await tradeService.GetAsync(id);

            if (item == null)
            {
                return NotFound(id);
            }

            return new ObjectResult(item);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Generate()
        {
            var item = await tradeService.GenerateAsync();
            return CreatedAtAction("Generate", new { id = item.Id }, item);
        }

        [HttpPost]
        public async Task<IActionResult> Post(AssetTradeViewModel trade)
        {
            var item = await tradeService.SaveAsync(trade);
            return CreatedAtAction("Post", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AssetTradeViewModel trade)
        {
            if (await tradeService.GetAsync(id) == null) return NotFound(id);

            var item = await tradeService.UpdateAsync(id, trade);
            return new OkObjectResult(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (await tradeService.GetAsync(id) == null) return NotFound(id);

            var item = await tradeService.DeleteAsync(id);
            return new OkObjectResult(item);
        }

        [HttpGet("Assets")]
        public async Task<IActionResult> Assets()
        {
            return Ok(await assetService.GetAllAsync());
        }
    }
}
