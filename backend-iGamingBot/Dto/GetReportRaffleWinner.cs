using backend_iGamingBot.Infrastructure;

namespace backend_iGamingBot.Dto
{
    public class GetReportRaffleWinner : GetSubParticipant
    {
        [ColumnLabel("Telegram ID")]
        public string TgId { get; set; } = null!;
        [ColumnLabel("Email")]
        public string? Email { get; set; } = null!;
        [ColumnLabel("Первое имя TG")]
        public string FirstName { get; set; } = null!;
    }
}
