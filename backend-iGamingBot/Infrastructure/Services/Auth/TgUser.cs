using System.Text.Json.Serialization;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class TgUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; } = string.Empty;

        [JsonPropertyName("allows_write_to_pm")]
        public bool AllowsWriteToPm { get; set; }
    }
}
