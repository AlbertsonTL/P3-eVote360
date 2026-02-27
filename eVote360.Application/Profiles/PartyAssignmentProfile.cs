using AutoMapper;
using eVote360.Domain.Entities.Assignments;
using eVote360.Application.DTOs.Response;

namespace eVote360.Application.Profiles;

public class PartyAssignmentProfile : Profile
{
    public PartyAssignmentProfile()
    {
        CreateMap<PartyAssignments, PartyAssignmentResponseDto>()
            .ForMember(dest => dest.UsuarioNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
            .ForMember(dest => dest.PartySiglas, opt => opt.MapFrom(src => src.Party.Siglas));
    }
}