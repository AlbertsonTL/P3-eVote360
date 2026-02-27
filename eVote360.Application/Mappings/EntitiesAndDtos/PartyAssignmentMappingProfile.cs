using AutoMapper;
using eVote360.Application.DTOs.Response;
using eVote360.Domain.Entities.Assignments;

namespace eVote360.Application.Mappings.EntitiesAndDtos;

public class PartyAssignmentMappingProfile : Profile
{
    public PartyAssignmentMappingProfile()
    {
        CreateMap<PartyAssignments, PartyAssignmentResponseDto>()
            .ForMember(d => d.UsuarioId, o => o.MapFrom(s => s.UsuarioId))
            .ForMember(d => d.UsuarioNombre, o => o.MapFrom(s => s.Usuario != null ? $"{s.Usuario.Nombre} {s.Usuario.Apellido}" : ""))
            .ForMember(d => d.UserNombre, o => o.MapFrom(s => s.Usuario != null ? $"{s.Usuario.Nombre} {s.Usuario.Apellido}" : ""))
            .ForMember(d => d.PartyId, o => o.MapFrom(s => s.PartyId))
            .ForMember(d => d.PartySiglas, o => o.MapFrom(s => s.Party != null ? s.Party.Siglas : ""))
            .ForMember(d => d.PartidoSiglas, o => o.MapFrom(s => s.Party != null ? s.Party.Siglas : ""))
            .ForMember(d => d.PartidoNombre, o => o.MapFrom(s => s.Party != null ? s.Party.Nombre : ""));
    }
}
