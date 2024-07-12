namespace backend_iGamingBot.Models.Essentials
{
    public class DefaultLiveParameter 
    {
        public bool IsLive { get; set; }
        public string? Link { get; set; }
        public string? Identifier { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is DefaultLiveParameter another)
            {
                return another.IsLive == this.IsLive && 
                    another.Link == this.Link && 
                    another.Identifier == this.Identifier;
            }
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return IsLive ? 1 : 0 + Link == null ? 0 : (int)Link!.Last() + Identifier == null ? 0 : 5;
        }
    }
}
