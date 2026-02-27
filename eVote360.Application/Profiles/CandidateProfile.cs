using AutoMapper;
using eVote360.Application.DTOs.Response;
using eVote360.Domain.Entities.Candidate;

namespace eVote360.Application.Profiles;

public class CandidateProfile : Profile
{
    public CandidateProfile()
    {
        CreateMap<Candidate, CandidateResponseDto>()
            .ForMember(dest => dest.PartyNombre, opt => opt.MapFrom(src => src.Party != null ? src.Party.Nombre : ""))
            .ForMember(dest => dest.PositionNombre, opt => opt.MapFrom(src => src.Position != null ? src.Position.Nombre : null));
    }
}
