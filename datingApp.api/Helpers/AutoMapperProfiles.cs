using System.Linq;
using AutoMapper;
using datingApp.api.DTOs;
using datingApp.api.Models;

namespace datingApp.api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url))
                                             .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            
            CreateMap<User, UserForDetailDto>().ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault( p => p.IsMain).Url))
                                             .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            
            CreateMap<Photo, PhotoForDetailDto>();
            
            CreateMap<UserForUpdateDto, User>();
            
            CreateMap<Photo, PhotoToReturnDTO>(); 
            
            CreateMap<PhotoToStoreDTO, Photo>(); 
            
        }
    }
}