using Microsoft.Win32.SafeHandles;
using System.Linq.Expressions;

namespace backend_iGamingBot.Infrastructure.Services
{
    public static class UserResolver
    {
        public static Expression<Func<DefaultUser, string?>> DefineImageUrl =>
            y => ExtractFilePath(y.ImageUrl);
        public static Expression<Func<Streamer, string?>> DefineStreamerImageUrl =>
           y => ExtractFilePath(y.ImageUrl);
        public static Expression<Func<User, string?>> DefineUserImageUrl =>
           y => ExtractFilePath(y.ImageUrl);
       public static Expression<Func<Subscriber, string?>> DefineSubscriberImageUrl =>
           y => ExtractFilePath(y.User!.ImageUrl);
        public static string? ExtractFilePath(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;
            string pattern = AppConfig.GlobalInstance.TgFilePath;
            int startIndex = url.IndexOf('/', pattern.Length + 1) + 1;
            string filePath = url.Substring(startIndex);
            var api = $"{AppConfig.GlobalInstance.Frontend}/api/user/file/{filePath}";
            return api;
        }
    }
}
