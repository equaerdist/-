using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaffleController : ControllerBase
    {
        private readonly IRaffleRepository _raffleSrc;
        private readonly IStreamerService _streamerSrv;

        public RaffleController(IRaffleRepository raffleSrc, IStreamerService streamerSrv) 
        {
            _raffleSrc = raffleSrc;
            _streamerSrv = streamerSrv;
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
    }
}
