using AutoMapper;
using backend_iGamingBot.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Text;
using SHA3.Net;

namespace backend_iGamingBot.Infrastructure.Services
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userSrc;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private static readonly Regex EmailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        public UserService(IUserRepository userSrc, 
            IUnitOfWork uof, IMapper mapper)
        {
            _userSrc = userSrc;
            _mapper = mapper;
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
                UserPayMethods = AppDictionary.DefaultPayMethods,
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
                UserPayMethods= AppDictionary.DefaultPayMethods,
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
        private static void ValidateTetherTRC20Address(string address)
        {
            var result = !string.IsNullOrEmpty(address) && address.StartsWith('T') && address.Length == 34;
            if (!result)
                throw new AppException(AppDictionary.TRC20NotCorrectAddress);
        }

        private static void ValidateTetherERC20Address(string address)
        {
            var result = Regex.IsMatch(address, "^0x[a-fA-F0-9]{40}$");
            if (!result)
                throw new AppException(AppDictionary.ERC20NotCorrectAddress);
        }

        private static void ValidatePiastrixAddress(string address)
        {
            var result = !string.IsNullOrEmpty(address) && address.Length <= 100;
            if (!result)
                throw new AppException(AppDictionary.PstrxNotCorrectAddress);
        }

     
        private void ValidatePayMethods(GetUserPayMethod[] methods)
        {
            foreach (var method in methods)
            {
                string? address = method.Data;
                if (string.IsNullOrEmpty(address))
                    continue;
                switch (method.Platform)
                {
                    case "Tether TRC20":
                        ValidateTetherTRC20Address(address);
                        break;
                    case "Tether ERC20":
                        ValidateTetherERC20Address(address);
                        break;
                    case "Piastrix":
                         ValidatePiastrixAddress(address);
                        break;
                    default:
                        throw new ArgumentException("Unknown payment system");
                }
            }
        }
        public async Task UpdateUserData(GetUserProfile dto, string sourceId)
        {
            if (dto.TgId != sourceId)
                throw new AppException(AppDictionary.Denied);
            var user = ((await _userSrc.GetUserByIdAsync(dto.TgId)))!;
            ValidatePayMethods(dto.UserPayMethods.ToArray());
            if(dto.Email != null)
                ValidateEmail(dto.Email);
            _mapper.Map(dto, user);
            await _uof.SaveChangesAsync();
        }

        private void ValidateEmail(string email)
        {
            if (!EmailRegex.IsMatch(email))
                throw new AppException(AppDictionary.InvalidEmail);
        }
    }
}
