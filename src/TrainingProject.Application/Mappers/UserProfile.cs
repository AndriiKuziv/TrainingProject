using AutoMapper;
using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;
using TrainingProject.Domain.Models;

namespace TrainingProject.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateUserRequest, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<User, CreateUserDto>();

        CreateMap<User, UpdateUserDto>();

        CreateMap<User, DeleteUserDto>();
    }
}
