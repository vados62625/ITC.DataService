using AutoMapper;
using ITC.Authorization.CQRS.InitialBrokerRegistrationRequests.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Suggest;

namespace ITC.Authorization.CQRS.InitialBrokerRegistrationRequests;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<InitialBrokerRegistrationRequest, InitialBrokerRegistrationRequestDto>();
        CreateMap<InitialBrokerRegistrationRequest, SuggestResultDto>()
            .ForMember(dest => dest.Value,
                o => o.MapFrom(x => $"{x.UserId}"));
    }
}