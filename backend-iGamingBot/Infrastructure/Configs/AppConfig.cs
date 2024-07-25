using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Text;

namespace backend_iGamingBot.Infrastructure
{
    public class AppConfig
    {
        public string SqlKey { get; init; } = null!;
        public string TwitchClientId { get; init; } = null!;
        public string TwitchSecretToken { get; init; } = null!;
        public string TgKey { get; set; } = null!;
        public static int DELAY_PER_REQUEST = 2000;
        public static short USER_BATCH_SIZE = 100;
        public string KEY { get; set; } = null!;
        public TimeSpan Expires => TimeSpan.FromDays(1);
        public SymmetricSecurityKey SymmetricSecurityKey =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        public string Frontend { get; set; } = null!;
        public string FrontendWww {  get; set; } = null!;
        public string FrontendCom { get; set; } = null!;
        public static short MinimalLengthForText => 10;
        public string TgFilePath => "https://api.telegram.org/file/bot";
        public static string LOCAL = "Development";
        public string ASPNETCORE_ENVIRONMENT { get; set; } = null!;
        public static AppConfig GlobalInstance { get; set; } = null!;
    }
}
