using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class StreamerController : Controller
    {
        private readonly IStreamerRepository _streamerSrc;

        public StreamerController(IStreamerRepository streamerSrc) 
        {
            _streamerSrc = streamerSrc;
        }
        [HttpGet("{tgId:string}")]
        public async Task<IActionResult> GetUserByTgId([FromRoute]string tgId)
        {
            var streamer = await _streamerSrc.GetStreamerByTgIdAsync(tgId);
            return Ok(streamer);
        }
        [HttpGet("{id:string}/raffles")]
        public async Task<IActionResult> GetRafflesAsync([FromQuery]int page, [FromQuery]int pageSize, 
            [FromQuery]string type, [FromRoute] string id)
        {
            var result = await _streamerSrc.GetRafflesAsync(page, pageSize, type, id);
            return Ok(result);
        }
        [HttpGet("{id:string}/subscribers")]
        public async Task<IActionResult> GetSubscribersAsync([FromQuery] int page, 
            [FromQuery] int pageSize, [FromRoute] string id)
        {
            var result = await _streamerSrc.GetSubscribersAsync(page, pageSize, id);
            return Ok(result);
        }
    }
}
