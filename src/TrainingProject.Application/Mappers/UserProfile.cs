using AutoMapper;
using TrainingProject.Application.Dtos;
using TrainingProject.Application.Requests;
using TrainingProject.Domain.Models;

namespace TrainingProject.Application.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ReverseMap();
        CreateMap<CreateUserRequest, User>();
        CreateMap<UpdateUserRequest, User>();
    }
}
