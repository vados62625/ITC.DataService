using AutoMapper;
using ITC.Authorization.CQRS.UserRolesCaches.Dto;
using ITC.Authorization.Storage;
using ITC.CQRS.Base.Suggest;

namespace ITC.Authorization.CQRS.UserRolesCaches;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<UserRolesCache, UserRolesCacheDto>();
        CreateMap<InitialBrokerRegistrationRequest, SuggestResultDto>()
            .ForMember(dest => dest.Value,
                o => o.MapFrom(x => $"{x.UserId}"));
    }
}