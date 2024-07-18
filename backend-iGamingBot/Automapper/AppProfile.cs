using AutoMapper;
using backend_iGamingBot.Dto;

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
        }
    }
}
