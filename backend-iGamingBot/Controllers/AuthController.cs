using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

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
        public async Task<IActionResult> Enter([FromBody] AuthRequest req)
        {
            var data = QueryHelpers.ParseQuery(req.Hash)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
            var token = await _auth.GetTokenAsync(data);
            Response.Cookies.Append("auth", token, new()
            {
                Expires = DateTime.UtcNow + _cfg.Expires,
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Secure = true
            });
            return Ok();
        }
        [HttpPost("local-enter")]
        public async Task<IActionResult> Enter()
        {
            var token = await _auth.GetTokenAsync(null!);
            Response.Cookies.Append("auth", token, new()
            {
                Expires = DateTime.UtcNow + _cfg.Expires,
                HttpOnly = false
            });
            return Ok();
        }
        [Authorize]
        [HttpGet]
        public IActionResult IsAuth() => Ok();
    }
}
