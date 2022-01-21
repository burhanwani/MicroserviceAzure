using AutoMapper;
using CommandService.Dtos;
using CommandService.Models;
//using PlatformService;

namespace CommandService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // Source -> Target
            CreateMap<Platformm, PlatformreadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, CommandReadDto>();
            
            // Map PlatformPublishedDto to Platform where the Id from source maps to ExternalID in dest. 
            CreateMap<PlatformPublishedDto, Platformm>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
            /*
            CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
        */
        }
    }
}
