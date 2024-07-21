using backend_iGamingBot.Dto;
using System.Linq.Expressions;

namespace backend_iGamingBot.Infrastructure.Services
{
    public static class SubscriberStatResolver
    {
        public static Expression<Func<DefaultUser, SingleStat>> CountAbuseParticipantStat => 
            y => new SingleStat(y.ParticipantNotes.Count(n => n.HaveAbused && n.Raffle!.WinnersDefined),
                        y.ParticipantNotes.Count(n => n.HaveAbused && n.Raffle!.WinnersDefined) 
                / (y.ParticipantNotes.Count(n => n.Raffle!.WinnersDefined) + 10E-5));
        public static Expression<Func<DefaultUser, SingleStat>> CountWons =>
            y => new SingleStat(y.WinnerRaffles.Count, y.WinnerRaffles.Count
                / (y.ParticipantNotes.Count(s => s.Raffle!.WinnersDefined) + 10E-5));
        public static Expression<Func<DefaultUser, SingleStat>> CountParticipants =>
            y => new SingleStat(y.ParticipantNotes.Count(n => n.Raffle!.WinnersDefined), null);
        public static Expression<Func<Subscriber, SubscriberStat>> DefineSubStats =>
            y => new SubscriberStat()
            {
                SpottedInAbusing = new SingleStat(y.User!.ParticipantNotes.Count(n => n.HaveAbused 
                && n.Raffle!.WinnersDefined),
                        y.User!.ParticipantNotes.Count(n => n.HaveAbused && n.Raffle!.WinnersDefined)
                / (y.User!.ParticipantNotes.Count(n => n.Raffle!.WinnersDefined) + 10E-5)),

                Participated = new SingleStat(y.User!.ParticipantNotes
                    .Count(n => n.Raffle!.WinnersDefined), null),

                Won = new SingleStat(y.User!.WinnerRaffles.Count, y.User!.WinnerRaffles.Count
                / (y.User!.ParticipantNotes.Count(n => n.Raffle!.WinnersDefined) + 10E-5)),

                ParticipatedInStreamer = new SingleStat(y.User!.ParticipantNotes
                    .Count(n => n.Raffle!.CreatorId == y.StreamerId && n.Raffle.WinnersDefined), null),

                SpottedInStreamerAbusing = new SingleStat(y.User!.ParticipantNotes
                    .Count(n => n.HaveAbused && n.Raffle!.CreatorId == y.StreamerId 
                    && n.Raffle!.WinnersDefined),
                        y.User!.ParticipantNotes.Count(n => n.HaveAbused 
                        && n.Raffle!.CreatorId == y.StreamerId && n.Raffle!.WinnersDefined)
                / (y.User!.ParticipantNotes.Count(n => n.Raffle!.CreatorId == y.StreamerId && n.Raffle!.WinnersDefined) + 10E-5)),

                WonStreamer = new SingleStat(y.User!.WinnerRaffles.Count(r => r.CreatorId == y.StreamerId), 
                    y.User!.WinnerRaffles.Count(r => r.CreatorId == y.StreamerId)
                / (y.User!.ParticipantNotes.Count(n => n.Raffle!.CreatorId == y.StreamerId && n.Raffle!.WinnersDefined) + 10E-5))
            };


    }
}
