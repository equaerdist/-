using backend_iGamingBot.Infrastructure;

namespace backend_iGamingBot.Dto
{
    public class GetSubscriberDto
    {
        [ColumnLabel("Id")]
        public long Id { get; set; }
        [ColumnLabel("Telegram ID")]
        public string TgId { get; set; } = null!;
        [ColumnLabel("Первое имя")]
        public string FirstName { get; set; } = null!;
        [ColumnLabel("Второе имя")]
        public string? LastName { get; set; }
        [ColumnLabel("Время когда подписался")]
        public DateTime SubscribeTime { get; set; }
        [ColumnLabel("Фото профиля")]
        public string? ImageUrl { get; set; }
        [ColumnLabel("Имя пользователя")]
        public string? Username { get; set; }
    }
}
