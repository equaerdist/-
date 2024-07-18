
namespace backend_iGamingBot.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userSrc;

        public UserService(IUserRepository userSrc)
        {
            _userSrc = userSrc;
        }
        private bool CheckWhenUserHaveEmail(DefaultUser user)
        {
            return user.Email != null;
        }
        public async Task<bool> ConditionIsDone(string description, string userId)
        {
            var user = await _userSrc.GetUserByIdAsync(userId);
            var validator = CheckWhenUserHaveEmail;
            return validator(user);
        }
    }
}
