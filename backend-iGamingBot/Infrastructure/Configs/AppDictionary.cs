
namespace backend_iGamingBot.Infrastructure
{
    public class AppDictionary
    {
        public static string YoutubeIdentifierNotFound => "Не удалось определить идентификатор пользователя";

        public static string YoutubeInitDataNotFound => "Не удалось найти объект инициализации ютуб";
        public static string ExceedLimit => "Можно отправить только не более 100 запросов за 1 раз";
        public static (string name, string pattern)[] ResolvedSocialNames => [
        
            ("YouTube",  @"^(http(s)?:\/\/)?(www\.)?youtube\.com\/@[\w-]+$"),
            ("Twitch", @"(http(s)?:\/\/)?(www\.)?twitch\.tv\/[a-zA-Z0-9_-]+"),
            ("Instagram", @"(http(s)?:\/\/)?(www\.)?instagram\.com\/[a-zA-Z0-9_.-]+"),
            ("Facebook", @"(http(s)?:\/\/)?(www\.)?facebook\.com\/[a-zA-Z0-9.]+"),
            ("TikTok", @"(http(s)?:\/\/)?(www\.)?tiktok\.com\/@?[a-zA-Z0-9_.-]+"),
            ("Telegram", @"(http(s)?:\/\/)?(www\.)?t\.me\/[a-zA-Z0-9_]+")
        ];

        public static string Denied => "Нет прав на выполнение операции";
        public static string UserNotExists => "Такого пользователя не существует";
        public static string TetherTRC20 => "Tether TRC20";
        public static string TetherERC20 => "Tether ERC20";
        public static string Piastrix => "Piastrix";
        public static UserPayMethod[] DefaultPayMethods =>
            [
                new() { Platform = TetherTRC20},
                new() { Platform = TetherERC20},
                new() { Platform = Piastrix}
            ];

        private static string GetSocialNameConstant(string social)
        {
            return AppDictionary.ResolvedSocialNames
                       .Where(s => s.name.Equals(social, StringComparison.OrdinalIgnoreCase))
                       .First().name;
        }
        public static string Youtube = GetSocialNameConstant(nameof(Youtube));
        public static string Twitch = GetSocialNameConstant(nameof(Twitch));
        public static string StreamerRole => "Streamer";
        public static string UserRole => "User";
        public static string EmailRaffleCondition => "Email";
        public static (string title, string description)[] ResolvedConditions => 
            [(EmailRaffleCondition, "Email для связи")];

        public static TimeSpan MinimalReserveForRaffleEnd => TimeSpan.FromMinutes(5);
        public static string ServerErrorOcurred => "Ошибка на сервере";

        public static string RaffleDescriptionNotEmpty => "Поле с описанием " +
            "не должно быть пустым или слишком маленьким";
        public static string RaffleEndTimeTooSoon => $"Конец розыгрыша не может быть менее чем " +
            $"через {MinimalReserveForRaffleEnd.TotalMinutes} минут после его начала";
        public static string RaffleAmountWinnersTooSmall => "В розыгрыше требуется как минимум 1 победитель";

        public static string OpenWebApp => "Открыть игровое приложение";

        public static string WelcomeMessage => "Рады Вас видеть!";

        public static string TelegramUserNotDefined => "Не смогли определить Ваши данные телеграм :(..";
        public static string UserAlreadyExists => "Стример или пользователь уже существуют";

        public static string PostBodyNotEmpty => "Текст в посте не должен быть пустым";

        public static string RaffleTimeExceeded => "Розыгрыш уже прошел";

        public static string NotResolvedSocial => "Данная соцсеть не поддерживается";

        public static string NotHaveAccess => "Недостаточно прав доступа";
        public static string NameId => "NameId";
        public static string Name => "Name";
        public static string Role => "Role";

        public static string? TRC20NotCorrectAddress => "Некорректный адрес для TRC20";
        public static string? ERC20NotCorrectAddress => "Некорректный адрес для ERC20";
        public static string? PstrxNotCorrectAddress => $"Некорректный адрес для {Piastrix}";

        public static string? InvalidEmail => "Неверный email формат";
        public static string Abused => "Abused";
        public static string Winner => "Winner";
        public static string Participant => "Participant";

        public static string? StreamerAlreadyExists => "Стример с таким именем уже существует";

        public static string Image => "Image";
    }
}
