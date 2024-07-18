using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class StreamerController : Controller
    {
        private readonly IStreamerRepository _streamerSrc;
        private readonly IStreamerService _streamerSrv;

        public StreamerController(IStreamerRepository streamerSrc, IStreamerService streamerSrv) 
        {
            _streamerSrc = streamerSrc;
            _streamerSrv = streamerSrv;

        }
        [HttpGet]
        public async Task<IActionResult> GetStreamersPageAsync([FromQuery] int page, [FromQuery] int pageSize,
           [FromQuery] string userId)
        {
            var resultPage = await _streamerSrc.GetStreamersPageAsync(page, pageSize, userId);
            return Ok(resultPage);
        }
        [HttpGet("{tgId}")]
        public async Task<IActionResult> GetStreamerByTgId([FromRoute]string tgId, [FromQuery] string userId)
        {
            var streamer = await _streamerSrc.GetStreamerByTgIdAsync(tgId, userId);
            return Ok(streamer);
        }
        [HttpPut("{streamerId}/subscribers/{userId}")]
        public async Task<IActionResult> SubscribeToStreamer([FromRoute] string streamerId, [FromRoute] string userId)
        {
            await _streamerSrv.SubscribeToStreamerAsync(streamerId, userId);
            return Ok();
        }
        [HttpGet("{id}/raffles")]
        public async Task<IActionResult> GetRafflesAsync([FromQuery]int page, [FromQuery]int pageSize, 
            [FromQuery]string type, [FromRoute] string id, [FromQuery] string userId)
        {
            var result = await _streamerSrv.GetRafflesAsync(page, pageSize, type, id, userId);
            return Ok(result);
        }
       
        [HttpGet("{id}/subscribers")]
        public async Task<IActionResult> GetSubscribersAsync([FromQuery] int page, 
            [FromQuery] int pageSize, [FromRoute] string id)
        {
            var result = await _streamerSrc.GetSubscribersAsync(page, pageSize, id);
            return Ok(result);
        }
        [HttpGet("{id}/admins")]
        public async Task<IActionResult> GetAdminsAsync([FromRoute]string id)
        {
            var result = await _streamerSrc.GetAdminsAsync(id);
            return Ok(result);
        }
    }
}
