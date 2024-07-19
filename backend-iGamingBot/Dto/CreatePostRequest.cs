namespace backend_iGamingBot.Dto
{
    public class CreatePostRequest
    {
        public IFormFile? Media { get; set; } = null!;
        public string? Message { get; set; } = null!;
    }
}
