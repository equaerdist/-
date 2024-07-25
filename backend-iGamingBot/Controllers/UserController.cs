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
        private readonly HttpClient _client;
        private readonly IUserRepository _userSrc;
        public string SourceId => User.Claims.First(c => c.Type == AppDictionary.NameId).Value;
        public UserController(IUserService userSrv, 
            IUserRepository userSrc, HttpClient client) 
        {
            _userSrv = userSrv;
            _client = client;
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
        private string GetContentType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".mp4" => "video/mp4",
                ".mkv" => "video/x-matroska",
                ".webm" => "video/webm",
                _ => "application/octet-stream",
            };
        }
        [HttpGet("file/{filePath}")]
        public async Task<IActionResult> GetProfilePhoto([FromRoute] string filePath)
        {
            var res = await _client.GetStreamAsync(
                $"{AppConfig.GlobalInstance.TgFilePath}" +
                $"{AppConfig.GlobalInstance.TgKey}/{filePath}");
            var contentType = GetContentType(filePath);
            var fileName = filePath.Split('/').Last();
            return File(res, contentType, fileName);
        }
        
    }
}
