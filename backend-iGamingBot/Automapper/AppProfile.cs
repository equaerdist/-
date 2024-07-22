﻿using AutoMapper;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure;
using backend_iGamingBot.Infrastructure.Services;
using backend_iGamingBot.Models;

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
                .ForMember(x => x.TgId, cfg => cfg.MapFrom(y => y.User!.TgId))
                .IncludeAllDerived();
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
            CreateMap<Subscriber, GetSubscriberProfile>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(y => y.UserId))
                .ForMember(x => x.TgId, cfg => cfg.MapFrom(y => y.User!.TgId))
                .ForMember(x => x.FirstName, cfg => cfg.MapFrom(y => y.User!.FirstName))
                .ForMember(x => x.Email, cfg => cfg.MapFrom(y => y.User!.Email))
                .ForMember(x => x.UserPayMethods, cfg => cfg.MapFrom(y => y.User!.UserPayMethods))
                .ForMember(x => x.LastName, cfg => cfg.MapFrom(y => y.User!.LastName))
                .ForMember(x => x.SubscriberStat, cfg => 
                    cfg.MapFrom(SubscriberStatResolver.DefineSubStats));
            CreateMap<ParticipantNote, GetSubParticipant>()
                .ForMember(x => x.Id, cfg => cfg.MapFrom(y => y.Raffle!.Id))
                .ForMember(x => x.EndTime, cfg => cfg.MapFrom(y => y.Raffle!.EndTime))
                .ForMember(x => x.Status, cfg => cfg.MapFrom(y => y.HaveAbused ? AppDictionary.Abused
                    : y.Raffle!.Winners.Select(u => u.TgId).Contains(y.Participant!.TgId) 
                    ?  AppDictionary.Winner : AppDictionary.Participant ))
                .IncludeAllDerived();
            CreateMap<ParticipantNote, GetReportRaffleWinner>()
                .ForMember(x => x.Email, cfg => cfg.MapFrom(y => y.Participant!.Email))
                .ForMember(x => x.FirstName, cfg => cfg.MapFrom(y => y.Participant!.FirstName))
                .ForMember(x => x.TgId, cfg => cfg.MapFrom(y => y.Participant!.TgId));
        }
    }
}
