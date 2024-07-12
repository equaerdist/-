using System.Text.Json.Serialization;

namespace backend_iGamingBot.Infrastructure.Services;
public class TelegramAuthDateDto
{
    [JsonPropertyName("auth_date")]
    public string AuthDate { get; set; } = null!;
    [JsonPropertyName("query_id")]
    public string QueryId { get; set; } = null!;
    [JsonPropertyName("user")]
    public string User { get; set; } = null!;
    [JsonPropertyName("hash")]
    public string Hash { get; set; } = null!;
}

