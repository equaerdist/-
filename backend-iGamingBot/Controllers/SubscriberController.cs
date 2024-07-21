using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly ISubscriberRepository _subSrc;

        public SubscriberController(ISubscriberRepository subSrc) 
        { 
            _subSrc = subSrc;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubscriberByTgId([FromRoute] string id, 
            [FromQuery] string streamerId)
        {
            var result = await _subSrc.GetSubProfileByTgId(id, streamerId);
            return Ok(result);
        }
        [HttpPost("{id}/message")]
        public async Task<IActionResult> SendSubMessage([FromRoute] string id, 
            [FromQuery] string streamerId)
        {

        }
    }
}
