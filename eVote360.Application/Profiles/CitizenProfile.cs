using AutoMapper;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.ViewModels.Citizens;
using eVote360.Domain.Entities.Citizen;
using eVote360.Domain.ValueObjects;

namespace eVote360.Application.Profiles;

public class CitizenProfile : Profile
{
    public CitizenProfile()
    {
        CreateMap<Citizen, CitizenResponseDto>()
            .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src => src.NumeroDocumento.Value))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value));

        CreateMap<CitizenCreateRequestDto, Citizen>()
            .ForMember(dest => dest.NumeroDocumento, opt => opt.MapFrom(src => new NationalId(src.NumeroDocumento)))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => new EmailAddress(src.Email)));

        CreateMap<CitizenVM, CitizenCreateRequestDto>();
        CreateMap<CitizenVM, CitizenUpdateRequestDto>();
        CreateMap<CitizenResponseDto, CitizenVM>();
    }
}
