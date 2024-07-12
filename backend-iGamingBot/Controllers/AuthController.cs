using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuth _auth;
        private readonly AppConfig _cfg;

        public AuthController(IAuth auth, AppConfig cfg) 
        {
            _auth = auth;
            _cfg = cfg;
        }
        [HttpPost]
        public IActionResult Enter([FromBody] TelegramAuthDateDto dto)
        {
            var token = _auth.GetToken(dto);
            Response.Cookies.Append("auth", token, new()
            {
                Expires = DateTime.UtcNow + _cfg.Expires,
                HttpOnly = true
            });
            return Ok();
        }
        [Authorize]
        [HttpGet]
        public IActionResult IsAuth() => Ok();
    }
}
