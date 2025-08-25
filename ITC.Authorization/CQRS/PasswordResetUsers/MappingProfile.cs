using AutoMapper;
using ITC.Authorization.CQRS.PasswordResetUsers.Dto;
using ITC.Authorization.Storage;

namespace ITC.Authorization.CQRS.PasswordResetUsers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Add, PasswordResetUser>();
        CreateMap<Get, PasswordResetUser>();
        CreateMap<Get, PasswordResetUserDto>();
        CreateMap<PasswordResetUser, PasswordResetUserDto>();
        CreateMap<PasswordResetUserDto, PasswordResetUser>();
    }
}