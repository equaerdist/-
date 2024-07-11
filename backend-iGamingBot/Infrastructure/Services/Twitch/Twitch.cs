
using System.Text.Json.Serialization;

namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public record AccessTokenResponse(
     [property: JsonPropertyName("access_token")] string AccessToken,
     [property: JsonPropertyName("expires_in")] string ExpiresIn,
     [property: JsonPropertyName("token_type")] string TokenType
    );

    public class Twitch : ITwitch
    {
        private readonly HttpClient _client;
        private readonly AppConfig _cfg;
        private static string TwitchCfgKey = "Twitch";

        private async Task<AccessTokenResponse> GetAccessToken()
        {
            var url = $"https://id.twitch.tv/oauth2/token?" +
                $"client_id={_cfg.TwitchClientId}&" +
                $"client_secret={_cfg.TwitchSecretToken}&" +
                $"grant_type=client_credentials";
            var res = await _client.PostAsync(url, null);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<AccessTokenResponse>() ?? throw new ArgumentNullException();
        }
        public Twitch(HttpClient client, AppConfig cfg) 
        {
            _client = client;
            _cfg = cfg;
        }
        public Task CheckUsersInOnline()
        {
            throw new NotImplementedException();
        }

        public Task<Config?> GetConfig()
        {
            throw new NotImplementedException();
        }

        public Task<Config> SetupConfig()
        {
            throw new NotImplementedException();
        }
    }
}
