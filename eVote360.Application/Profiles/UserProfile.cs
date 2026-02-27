using AutoMapper;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.ViewModels.Users;
using eVote360.Domain.Entities;
using eVote360.Domain.ValueObjects;

namespace eVote360.Application.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<Usuario, UserResponseDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<UserCreateRequestDto, Usuario>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => new EmailAddress(src.Email)));

        CreateMap<UserVM, UserCreateRequestDto>();
        CreateMap<UserVM, UserUpdateRequestDto>();
        CreateMap<UserResponseDto, UserVM>();
    }
}
