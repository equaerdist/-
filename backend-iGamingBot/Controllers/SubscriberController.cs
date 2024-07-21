using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly ISubscriberRepository _subSrc;
        private readonly ISubscriberService _subSrv;

        public string SourceId => User.Claims.First(c => c.Type == AppDictionary.NameId).Value;

        public SubscriberController(ISubscriberRepository subSrc, ISubscriberService subSrv) 
        { 
            _subSrc = subSrc;
            _subSrv = subSrv;
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
            [FromBody] SendSubMessageRequest req)
        {
            await _subSrv.SendSubMessage(req, SourceId);
            return Ok();
        }
        [HttpPost("{id}/note")]
        public async Task<IActionResult> EditSubNote(EditNoteAboutSub req)
        {
            await _subSrv.EditNoteAboutSub(req, SourceId);
            return Ok();
        }
        [HttpGet("{id}/participants")]
        public async Task<IActionResult> GetParticipants([FromRoute] string id,
            [FromQuery] string streamerId, [FromQuery] int page, [FromQuery] int pageSize)
        {
            var result = await _subSrc.GetSubParticipants(id, streamerId, page, pageSize);
            return Ok(result);
        }
    }
}
