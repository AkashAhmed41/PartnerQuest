using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Models;
using BackendWebApi.Extensions;

namespace BackendWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, MemberDataflow>()
                .ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsProfilePhoto).PhotoUrl))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<Photo, PhotoDataflow>();
        }
    }
}