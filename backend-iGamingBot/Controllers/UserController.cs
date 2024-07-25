using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userSrv;
        private readonly IUserRepository _userSrc;
        public string SourceId => User.Claims.First(c => c.Type == AppDictionary.NameId).Value;
        public UserController(IUserService userSrv, 
            IUserRepository userSrc) 
        {
            _userSrv = userSrv;
            _userSrc = userSrc;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserByTgId([FromRoute] string id)
        {
            var user = await _userSrc.GetUserProfileByTgIdAsync(id);
            return Ok(user);
        }
        [HttpPost("{id}")]
        public async Task<IActionResult> UpdateUserByTgId([FromRoute] string id, 
            [FromBody]GetUserProfile dto)
        {
            await _userSrv.UpdateUserData(dto, SourceId);
            return Ok();
        }
       
    }
}
