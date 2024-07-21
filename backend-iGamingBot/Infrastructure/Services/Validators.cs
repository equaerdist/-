using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public static class Validators
    {
        public static  void ValidatePostRequest(CreatePostRequest req)
        {
            if (string.IsNullOrEmpty(req.Message) || req.Message.Length < AppConfig.MinimalLengthForText)
                throw new AppException(AppDictionary.PostBodyNotEmpty);
        }
    }
}
