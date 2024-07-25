using backend_iGamingBot.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly HttpClient _client;

        public PhotosController(HttpClient client) 
        {
            _client = client;
        }
        [HttpGet("{filePath}")]
        public async Task<IActionResult> GetPhotoByName([FromRoute] string filePath)
        {
            var res = await _client.GetStreamAsync(
               $"{AppConfig.GlobalInstance.TgFilePath}" +
               $"{AppConfig.GlobalInstance.TgKey}/photos/{filePath}");
            var contentType = GetContentType(filePath);
            var fileName = filePath.Split('/').Last();
            return File(res, contentType, fileName);
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
    }
}
