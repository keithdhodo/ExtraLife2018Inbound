using AutoMapper;
using EL.DonorDrive.Entities;

namespace ExtraLife2018InboundETL.Entities
{
    public static class MapInitializer
    {
        private static bool activated;

        public static void Activate()
        {
            if (!activated)
            { 
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<ParticipantDto, Participant>()
                        .ReverseMap();
                    cfg.CreateMap<ParticipantTableEntity, ParticipantDto>()
                        .ForMember(dest => dest.EventId,
                            opt => opt.ResolveUsing(src =>
                            {
                                uint.TryParse(src.EventId, out var eventId);
                                return eventId;
                            }))
                        .ForMember(dest => dest.FundraisingGoal,
                            opt => opt.MapFrom(src => decimal.Parse(src.FundraisingGoal)))
                        .ForMember(dest => dest.IsTeamCaptain,
                            opt => opt.ResolveUsing(src =>
                            {
                                bool.TryParse(src.IsTeamCaptain, out var isTeamCaptain);
                                return isTeamCaptain;
                            }))
                        .ForMember(dest => dest.NumberOfDonations,
                            opt => opt.MapFrom(src => uint.Parse(src.NumberOfDonations)))
                        .ForMember(dest => dest.ParticipantId,
                            opt => opt.MapFrom(src => uint.Parse(src.ParticipantId)))
                        .ForMember(dest => dest.SumDonations,
                            opt => opt.MapFrom(src => double.Parse(src.SumDonations)))
                        .ForMember(dest => dest.TeamId,
                            opt => opt.ResolveUsing(src =>
                            {
                                uint.TryParse(src.TeamId, out var teamId);
                                return teamId;
                            }))
                        .ReverseMap();
                    cfg.CreateMap<DonationTableEntity, Donor>()
                        .ForMember(dest => dest.Amount,
                            opt => opt.ResolveUsing(src =>
                            {
                                double.TryParse(src.Amount, out var amount);
                                return amount;
                            }))
                        .ForMember(dest => dest.ParticipantId,
                            opt => opt.ResolveUsing(src =>
                            {
                                uint.TryParse(src.ParticipantId, out var participantId);
                                return participantId;
                            }))
                        .ForMember(dest => dest.TeamId,
                            opt => opt.ResolveUsing(src =>
                            {
                                uint.TryParse(src.TeamId, out var teamId);
                                return teamId;
                            }))
                        .ReverseMap();
                });

                activated = true;
            }
        }
    }
}
