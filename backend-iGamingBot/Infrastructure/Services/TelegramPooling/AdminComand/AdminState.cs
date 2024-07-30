namespace backend_iGamingBot.Infrastructure.Services
{
    public enum AdminStep {
        AwaitingName,
        Completed
    }
    public class AdminState
    {
        public AdminStep AdminStep { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
