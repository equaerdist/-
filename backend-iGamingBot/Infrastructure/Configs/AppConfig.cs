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
        public string KEY { get; set; } = null!;
        public TimeSpan Expires => TimeSpan.FromDays(1);
        public SymmetricSecurityKey SymmetricSecurityKey =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
