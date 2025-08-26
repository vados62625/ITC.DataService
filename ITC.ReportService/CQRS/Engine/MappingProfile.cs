using AutoMapper;
using ITC.Domain.Dto;

namespace ITC.ReportService.CQRS.Engine;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Domain.Models.Engine, EngineDto>();
        CreateMap<AddCommand, Domain.Models.Engine>();
        CreateMap<EngineDto, UpdateCommand>();
        CreateMap<UpdateCommand, Domain.Models.Engine>();
        CreateMap<EngineDto, UpdateCommand>();
    }
}