using backend_iGamingBot.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userSrc;
        private readonly IUnitOfWork _uof;

        public UserService(IUserRepository userSrc, IUnitOfWork uof)
        {
            _userSrc = userSrc;
            _uof = uof;
        }
        private bool CheckWhenUserHaveEmail(DefaultUser user)
        {
            return user.Email != null;
        }
        public async Task<bool> ConditionIsDone(string title, string userId)
        {
            var user = await _userSrc.GetUserByIdAsync(userId);
            var validator = CheckWhenUserHaveEmail;
            return validator(user);
        }

        public async Task<Streamer> RegisterStreamer(CreateStreamerRequest req)
        {
            var streamer = new Streamer()
            {
                Email = null,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Name = req.Name,
                TgId = req.TgId,
            };
            await _userSrc.AddUserAsync(streamer);
            try
            {
                await _uof.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            when (e.InnerException?.InnerException is SqlException sqlEx &&
             (sqlEx.Number == 2601 || sqlEx.Number == 2627))
            {
                throw new AppException(AppDictionary.UserAlreadyExists);
            }
            return streamer;
        }

        public async Task<User> RegisterUser(CreateUserRequest req)
        {
            var user = new User()
            {
                Email = null,
                FirstName = req.FirstName,
                LastName = req.LastName,
                TgId = req.TgId,
            };
            await _userSrc.AddUserAsync(user);
            await _uof.SaveChangesAsync();
            return user;
        }

        public async Task CheckUserInformation(CreateUserRequest req)
        {
            var user = await _userSrc.GetUserByIdAsync(req.TgId);
            if (req.FirstName != user.FirstName || req.LastName != user.LastName)
            {
                user.FirstName = req.FirstName;
                user.LastName = req.LastName;
                await _uof.SaveChangesAsync();
            }
        }
    }
}
