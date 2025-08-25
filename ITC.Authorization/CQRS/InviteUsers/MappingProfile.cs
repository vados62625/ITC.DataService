using AutoMapper;
using ITC.Authorization.CQRS.InviteUsers.Dto;
using ITC.Authorization.Storage;

namespace ITC.Authorization.CQRS.InviteUsers;

public class MappingProfile : Profile
{

    public MappingProfile()
    {
        CreateMap<Add, InvitedUser>();
        CreateMap<Get, InvitedUser>();
        CreateMap<Get, InvitedUserDto>();
        CreateMap<InvitedUser, InvitedUserDto>();
        CreateMap<InvitedUserDto, InvitedUser>();
    }
}