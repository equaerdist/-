namespace backend_iGamingBot
{
    public class Social
    {
        public Social()
        {
            Parameters = new List<ILiveParameter>();
        }
        public string Name { get; set; } = null!;
        public string Link { get; set; } = null!;
        public List<ILiveParameter> Parameters { get; set; }
    }
}
