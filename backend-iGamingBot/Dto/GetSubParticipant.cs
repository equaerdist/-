using backend_iGamingBot.Infrastructure;

namespace backend_iGamingBot.Dto
{
    public class GetSubParticipant
    {
        [ColumnLabel("ID")]
        public long Id { get; set; }
        [ColumnLabel("Статус участия")]
        public string Status { get; set; } = null!;
        [ColumnLabel("Время окончания")]
        public DateTime EndTime { get; set; }
    }
}
