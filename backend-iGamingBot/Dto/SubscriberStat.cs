namespace backend_iGamingBot.Dto
{
    public record class SingleStat(int amount, double? percentage);
    public class SubscriberStat
    {
        public SingleStat ParticipatedInStreamer { get; set; } = null!;
        public SingleStat WonStreamer { get; set; } = null!;
        public SingleStat SpottedInStreamerAbusing { get; set; } = null!;
        public SingleStat Participated { get; set; } = null!;
        public SingleStat Won{ get; set; } = null!;
        public SingleStat SpottedInAbusing { get; set; } = null!;
    }
}
