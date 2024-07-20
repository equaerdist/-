using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace backend_iGamingBot.Infrastructure
{
    public class AppConfig
    {
        public string SqlKey { get; init; } = null!;
        public string TwitchClientId { get; init; } = null!;
        public string TwitchSecretToken { get; init; } = null!;
        public string Host { get; init; } = null!;
        public string TgKey { get; set; } = null!;
        public static int DELAY_PER_REQUEST = 2000;
        public static short USER_BATCH_SIZE = 100;
        public string KEY { get; set; } = null!;
        public TimeSpan Expires => TimeSpan.FromDays(1);
        public SymmetricSecurityKey SymmetricSecurityKey =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        public static string Frontend => "https://localhost:5173";
        public static short MinimalLengthForText => 10;
        public static string LOCAL = "LOCAL";
        public static string Environment => LOCAL;
    }
}
