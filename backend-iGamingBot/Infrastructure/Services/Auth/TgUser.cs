using System.Text.Json.Serialization;

namespace backend_iGamingBot.Infrastructure.Services
{ 
    public class TgUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = null!;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = null!;

        [JsonPropertyName("username")]
        public string Username { get; set; } = null!;

        [JsonPropertyName("language_code")]
        public string LanguageCode { get; set; } = null!;

        [JsonPropertyName("allows_write_to_pm")]
        public bool AllowsWriteToPm { get; set; }
    }
}
