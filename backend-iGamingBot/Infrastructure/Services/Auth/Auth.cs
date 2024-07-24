using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class Auth : IAuth
    {
        private readonly AppConfig _cfg;
        private readonly IUserRepository _userSrc;

        public Auth(AppConfig cfg, IUserRepository userSrc) 
        {
            _cfg = cfg;
            _userSrc = userSrc;
        }
        private static byte[] HMAC_SHA256(byte[] data, byte[] key)
        {
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }
        private static string CombineData(Dictionary<string, string> data)
        {
            return string.Join("\n", data.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key}={kv.Value}"));
        }
        private Dictionary<string, string> ParseQueryString(string queryString)
        {
            var parsedData = QueryHelpers.ParseQuery(queryString);
            return parsedData.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());
        }
        public async Task<string> GetTokenAsync(Dictionary<string, string> data)
        {
            var nameId = string.Empty;
            var name = string.Empty;
            if (_cfg.ASPNETCORE_ENVIRONMENT != AppConfig.LOCAL)
            {
                string hash = data["hash"];
                data.Remove("hash");
                string dataString = CombineData(data);
                byte[] secret_key = HMAC_SHA256(Encoding.UTF8.GetBytes(_cfg.TgKey), Encoding.UTF8.GetBytes("WebAppData"));

                string calculatedHash = BitConverter.ToString(
                    HMAC_SHA256(Encoding.UTF8.GetBytes(dataString), secret_key))
                    .Replace("-", "");
                bool isValid = calculatedHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
                if (!isValid)
                    throw new AppException(AppDictionary.Denied);

                var tgUser = JsonSerializer.Deserialize<TgUser>(data["user"]) 
                    ?? throw new InvalidDataException();
                nameId = tgUser.Id.ToString();
                name = tgUser.FirstName;
            }
            else
            {
                nameId = "99999";
                name = "Peter";
            }
            var claims = new List<Claim>()
            {
                new(type: AppDictionary.NameId, value: nameId),
                new(type:AppDictionary.Name, value: name),
                new(type:AppDictionary.Role, value: await _userSrc.DefineRoleByTgIdAsync(nameId))
            };
            var jwt = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.Add(_cfg.Expires),
            signingCredentials: new SigningCredentials(_cfg.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
