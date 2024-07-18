
namespace backend_iGamingBot
{
    public class User : DefaultUser
    {
        public User() 
        {
            PayMethods = new();
        }
        public List<UserPayMethod> PayMethods { get; set; }
    }
}
