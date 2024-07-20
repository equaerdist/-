using AutoMapper;
using backend_iGamingBot.Dto;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;

namespace backend_iGamingBot.Automapper
{
    public class AppProfile : Profile
    {
        public AppProfile() 
        {
            CreateMap<Streamer, GetStreamerDto>()
                .ForMember(s => s.AmountOfSubscribers, cfg => cfg.MapFrom(y => y.Subscribers.Count()));
                
            CreateMap<Subscriber, GetSubscriberDto>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(y => y.UserId))
                .ForMember(x => x.FirstName, cfg => cfg.MapFrom(y => y.User!.FirstName))
                .ForMember(x => x.LastName, cfg => cfg.MapFrom(y => y.User!.LastName))
                .ForMember(x => x.TgId, cfg => cfg.MapFrom(y => y.User!.TgId));
            CreateMap<DefaultUser, GetAdminDto>();
            CreateMap<CreateRaffleRequest, Raffle>();
            CreateMap<Social, GetSocialDto>();
            CreateMap<GetSocialDto, Social>();
            CreateMap<User, GetUserProfile>();
            CreateMap<UserPayMethod, GetUserPayMethod>();
            CreateMap<GetUserPayMethod, UserPayMethod>();
            CreateMap<GetUserProfile, DefaultUser>()
                .ForMember(x => x.Id, cfg => cfg.Ignore())
                .ForMember(x => x.TgId, cfg => cfg.Ignore())
                .ForMember(x => x.FirstName, cfg => cfg.Ignore())
                .ForMember(x => x.LastName, cfg => cfg.Ignore());
        }
    }
}
