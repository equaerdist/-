using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class StreamerController : Controller
    {
        private readonly IStreamerRepository _streamerSrc;
        private readonly IStreamerService _streamerSrv;
        private readonly IRaffleRepository _raffleSrc;
        public string SourceId => User.Claims.First(c => c.Type == AppDictionary.NameId).Value;
        public StreamerController(IStreamerRepository streamerSrc, 
            IStreamerService streamerSrv,
            IRaffleRepository raffleSrc)
        {
            _streamerSrc = streamerSrc;
            _streamerSrv = streamerSrv;
            _raffleSrc = raffleSrc;

        }
        [HttpGet]
        public async Task<IActionResult> GetStreamersPageAsync([FromQuery] int page, [FromQuery] int pageSize,
           [FromQuery] string userId)
        {
            var resultPage = await _streamerSrc.GetStreamersPageAsync(page, pageSize, userId);
            return Ok(resultPage);
        }
        [HttpGet("{tgId}")]
        public async Task<IActionResult> GetStreamerByTgId([FromRoute] string tgId, [FromQuery] string userId)
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
        [HttpDelete("{streamerId}/subscribers/{userId}")]
        public async Task<IActionResult> UnsubscribeToStreamer([FromRoute] string streamerId, [FromRoute,] string userId)
        {
            await _streamerSrv.UnscribeFromStreamerAsync(streamerId, userId);
            return Ok();
        }
        [HttpGet("{id}/raffles")]
        public async Task<IActionResult> GetRafflesAsync([FromQuery] int page, [FromQuery] int pageSize,
            [FromQuery] string type, [FromRoute] string id, [FromQuery] string userId)
        {
            var result = await _streamerSrv.GetRafflesAsync(page, pageSize, type, id, userId);
            return Ok(result);
        }
        [HttpPost]
        [HttpPost("{id}/raffles")]
        public async Task<IActionResult> CreateRaffleAsync([FromBody]CreateRaffleRequest req, [FromRoute] string id)
        {
            var raffle = await _streamerSrv.CreateRaffleAsync(req, id, SourceId);
            return Ok(raffle);
        }
        [HttpGet("{id}/subscribers")]
        public async Task<IActionResult> GetSubscribersAsync([FromQuery] int page,
            [FromQuery] int pageSize, [FromRoute] string id)
        {
            var result = await _streamerSrc.GetSubscribersAsync(page, pageSize, id);
            return Ok(result);
        }
        [HttpGet("{id}/admins")]
        public async Task<IActionResult> GetAdminsAsync([FromRoute] string id)
        {
            var result = await _streamerSrc.GetAdminsAsync(id);
            return Ok(result);
        }
        [HttpPost("{id}/admins/{adminId}")]
        public async  Task<IActionResult> AddStreamerAdmin([FromRoute]string id, 
            [FromRoute]string adminId)
        {
            await _streamerSrv.AddStreamerAdmin(id, adminId, SourceId);
            return Ok();
        }
        [HttpGet("socials")]
        public IActionResult GetAvailableSocials()
        {
            return Ok(_streamerSrv.GetAvailableSocials());
        }
        [HttpGet("conditions")]
        public IActionResult GetAvailableConditions()
        {
            return Ok(AppDictionary.ResolvedConditions.Select(t => t.title).ToArray());
        }
        [Authorize]
        [HttpPost("{id}/posts")]
        public async Task<IActionResult> CreatePost([FromRoute] string id, [FromForm] CreatePostRequest req)
        {
            await _streamerSrv.CreatePostAsync(req, id, SourceId);
            return Ok();
        }
        [HttpGet("{id}/socials")]
        public async Task<IActionResult> GetStreamerSocials([FromRoute]string id)
        {
            var result = await _streamerSrc.GetStreamerSocials(id);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("{id}/socials")]
        public async Task<IActionResult> AddStreamerSocial([FromBody]GetSocialDto req, string id)
        {
            await _streamerSrv.AddStreamerSocial(req, id, SourceId);
            return Ok();
        }
        [Authorize]
        [HttpPost("{id}/report")]
        public async Task<IActionResult> GetStreamerReport([FromRoute] string id)
        {
            await _streamerSrv.CreateRequestForSubscribersReport(id, SourceId);
            return Ok();
        }

    }
}
