
namespace backend_iGamingBot
{
    public class Streamer
    {
        public Streamer() 
        { 
            Socials = new List<Social>();
        }
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Social> Socials { get; set; }
    }
}
