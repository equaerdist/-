using AutoMapper;
using backend_iGamingBot.Dto;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Text;
using SHA3.Net;
using backend_iGamingBot.Migrations;

namespace backend_iGamingBot.Infrastructure.Services
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userSrc;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uof;
        private readonly IStreamerRepository _streamerSrc;
        private readonly IRaffleRepository _raffleSrc;
        private static readonly Regex EmailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        private static readonly string _tgKeyConstraint = "IX_AllUsers_TgId";
        //private static readonly string _streamerNameConstraint = "IX_AllUsers_Name";

        public UserService(IUserRepository userSrc,
            IUnitOfWork uof, IMapper mapper,
            IStreamerRepository streamerSrc,
            IRaffleRepository raffleSrc)
        {
            _userSrc = userSrc;
            _mapper = mapper;
            _uof = uof;
            _streamerSrc = streamerSrc;
            _raffleSrc = raffleSrc;
        }
        private bool CheckWhenUserHaveEmail(DefaultUser user)
        {
            return user.Email != null && user.Email != string.Empty;
        }


        public async Task<Streamer> RegisterStreamer(CreateStreamerRequest req)
        {

            var handledName = req.Name.Replace("_", " ");
            var trueName = handledName.Split("-").First();
            if (!await _streamerSrc.StreamerInviteAlreadyExists(handledName))
                throw new AppException(AppDictionary.Denied);
            var streamer = new Streamer()
            {
                Email = null,
                FirstName = req.FirstName,
                LastName = req.LastName,
                Name = trueName,
                ImageUrl = req.ImageUrl,
                TgId = req.TgId,
                UserPayMethods = AppDictionary.DefaultPayMethods,
            };
            await _userSrc.AddUserAsync(streamer);
            try
            {
                await _uof.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            when (e.InnerException != null)
            {
                if (e.InnerException.Message.Contains(_tgKeyConstraint))
                {
                    await _userSrc.RemoveUserAsync(req.TgId);
                    await _uof.SaveChangesAsync();
                }
                else
                {
                    throw new AppException(AppDictionary.StreamerAlreadyExists);
                }
            }
            await _streamerSrc.RemoveStreamerInvite(trueName);
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
                UserPayMethods = AppDictionary.DefaultPayMethods,
                ImageUrl = req.ImageUrl
            };
            await _userSrc.AddUserAsync(user);
            await _uof.SaveChangesAsync();
            return user;
        }

        public async Task CheckUserInformation(CreateUserRequest req)
        {
            var user = await _userSrc.GetUserByIdAsync(req.TgId);
            if (req.FirstName != user.FirstName
                || req.LastName != user.LastName
                || req.ImageUrl != user.ImageUrl
                || req.Username != user.Username)
            {
                user.FirstName = req.FirstName;
                user.LastName = req.LastName;
                user.Username = req.Username;
                user.ImageUrl = req.ImageUrl;
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

        public async Task<string[]> RaffleConditionIsDone(long raffleId, string userId)
        {
            var raffle = await _raffleSrc.GetRaffleByIdAsync(raffleId);
            var user = await _userSrc.GetUserByIdAsync(userId);
            var result = new List<string>();
            foreach (var c in (List<string>)raffle.RaffleConditions)
            {
                var validator = CheckWhenUserHaveEmail;
                var resultCheck = validator(user);
                if (!resultCheck)
                    result.Add($"Условие {c} не выполнено");
            }
            return result.ToArray();
        }

        public async Task<bool> SingleRaffleConditionIsDone(string description, string userId)
        {
            var user = await _userSrc.GetUserByIdAsync(userId);
            var validator = CheckWhenUserHaveEmail;
            return validator(user);
        }
    }
}
