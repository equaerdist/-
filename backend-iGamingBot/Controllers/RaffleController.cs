using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleRepository _raffleSrc;
        private readonly IStreamerService _streamerSrv;
        private readonly IRaffleService _raffleSrv;
        public string SourceId => User.Claims.First(c => c.Type == AppDictionary.NameId).Value;
        public RaffleController(IRaffleRepository raffleSrc, 
            IStreamerService streamerSrv,
            IRaffleService raffleSrv) 
        {
            _raffleSrc = raffleSrc;
            _streamerSrv = streamerSrv;
            _raffleSrv = raffleSrv;
        }
        [HttpGet("{raffleId:long}")]
        public async Task<IActionResult> GetRaffleByIdAsync([FromRoute] long raffleId)
        {
            var result = await _raffleSrc.GetRaffleByIdAsync(raffleId);
            return Ok(result);
        }
        [HttpPut("{raffleId:long}/participants/{userId}")]
        public async Task<IActionResult> DoParticipantInRaffle([FromRoute] long raffleId, [FromRoute] string userId)
        {
            await _streamerSrv.DoParticipantInRaffleAsync(raffleId, userId);
            return Ok();
        }
        [HttpGet("{id:long}/winners")]
        public async Task<IActionResult> GetRaffleWinnersAsync([FromRoute] long id)
        {
            var result = await _raffleSrc.GetRaffleWinners(id);
            return Ok(result);
        }
        [Authorize]
        [HttpPost("{id:long}/winners")]
        public async Task<IActionResult> GenerateWinners([FromRoute] long id, GenerateWinnersRequest req)
        {
            await _raffleSrv.GenerateWinnersForRaffle(id, req.ExceptRepeats, SourceId, req.AmountOfWinners);
            return Ok();
        }
    }
}
