using AutoMapper;
using ITC.Domain.Dto;

namespace ITC.Domain.CQRS.Engine;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Models.Engine, EngineDto>();
        CreateMap<AddCommand, Models.Engine>();
        CreateMap<EngineDto, UpdateCommand>();
        CreateMap<UpdateCommand, Models.Engine>();
        CreateMap<EngineDto, UpdateCommand>();
    }
}