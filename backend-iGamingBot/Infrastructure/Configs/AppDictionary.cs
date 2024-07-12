﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace backend_iGamingBot.Infrastructure
{
    public class AppDictionary
    {
        public static string YoutubeIdentifierNotFound => "Не удалось определить идентификатор пользователя";

        public static string YoutubeInitDataNotFound => "Не удалось найти объект инициализации ютуб";
        public static string ExceedLimit => "Можно отправить только не более 100 запросов за 1 раз";
        public static (string name, string pattern)[] ResolvedSocialNames => new[]
        {
            ("YouTube", @"(http(s)?:\/\/)?(www\.)?youtube\.com\/@[a-zA-Z0-9_-]+"),
            ("Twitch", @"(http(s)?:\/\/)?(www\.)?twitch\.tv\/[a-zA-Z0-9_-]+"),
            ("Instagram", @"(http(s)?:\/\/)?(www\.)?instagram\.com\/[a-zA-Z0-9_.-]+"),
            ("Twitter", @"(http(s)?:\/\/)?(www\.)?twitter\.com\/[a-zA-Z0-9_]+"),
            ("Facebook", @"(http(s)?:\/\/)?(www\.)?facebook\.com\/[a-zA-Z0-9.]+"),
            ("LinkedIn", @"(http(s)?:\/\/)?(www\.)?linkedin\.com\/in\/[a-zA-Z0-9_-]+"),
            ("TikTok", @"(http(s)?:\/\/)?(www\.)?tiktok\.com\/@?[a-zA-Z0-9_.-]+"),
            ("GitHub", @"(http(s)?:\/\/)?(www\.)?github\.com\/[a-zA-Z0-9_-]+"),
            ("Reddit", @"(http(s)?:\/\/)?(www\.)?reddit\.com\/user\/[a-zA-Z0-9_-]+"),
            ("Pinterest", @"(http(s)?:\/\/)?(www\.)?pinterest\.com\/[a-zA-Z0-9_/]+")
        };

    }
}
