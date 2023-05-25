using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Models;

namespace BackendWebApi.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, MemberDataflow>().ForMember(dest => dest.ProfilePhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsProfilePhoto).PhotoUrl));
            CreateMap<Photo, PhotoDataflow>();
        }
    }
}