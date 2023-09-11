using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Models;
using BackendWebApi.Extensions;

namespace BackendWebApi.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        CreateMap<User, MemberDataflow>()
            .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsProfilePhoto).PhotoUrl))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
        CreateMap<Photo, PhotoDataflow>();
        CreateMap<EditMemberInfoDataflow, User>();
        CreateMap<RegisterDataflow, User>();
        CreateMap<Message, MessageDataflow>()
            .ForMember(dest => dest.SenderProfilePhotoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(x => x.IsProfilePhoto).PhotoUrl))
            .ForMember(d => d.RecipientProfilePhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(p => p.IsProfilePhoto).PhotoUrl));
        CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));
        CreateMap<DateTime?, DateTime?>().ConvertUsing(d => d.HasValue ? DateTime.SpecifyKind(d.Value, DateTimeKind.Utc) : null);
    }
}