using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace backend_iGamingBot.Infrastructure
{
    public class AppDictionary
    {
        public static string YoutubeIdentifierNotFound => "Не удалось определить идентификатор пользователя";

        public static string YoutubeInitDataNotFound => "Не удалось найти объект инициализации ютуб";
        public static string ExceedLimit => "Можно отправить только не более 100 запросов за 1 раз";
        public static (string name, string pattern)[] ResolvedSocialNames => new[]
        {
            ("YouTube", @"https://www\.youtube\.com/@[a-zA-Z0-9_-]+"),
            ("Twitch", @"https://www\.twitch\.tv/[a-zA-Z0-9_-]+"),
            ("Instagram", @"https://www\.instagram\.com/[a-zA-Z0-9_.-]+"),
            ("Twitter", @"https://twitter\.com/[a-zA-Z0-9_]+"),
            ("Facebook", @"https://www\.facebook\.com/[a-zA-Z0-9.]+"),
            ("LinkedIn", @"https://www\.linkedin\.com/in/[a-zA-Z0-9_-]+"),
            ("TikTok", @"https://www\.tiktok\.com/@[a-zA-Z0-9_.-]+"),
            ("GitHub", @"https://github\.com/[a-zA-Z0-9_-]+"),
            ("Reddit", @"https://www\.reddit\.com/user/[a-zA-Z0-9_-]+"),
            ("Pinterest", @"https://www\.pinterest\.com/[a-zA-Z0-9_/]+")
        };

    }
}
