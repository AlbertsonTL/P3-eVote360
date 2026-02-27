using AutoMapper;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.ViewModels.Party;
using eVote360.Domain.Entities.Party;

namespace eVote360.Application.Profiles;

public class PartyProfile : Profile
{
    public PartyProfile()
    {
        CreateMap<Party, PartyResponse>();
        CreateMap<PartyCreateRequest, Party>();
        CreateMap<PartyFormVM, PartyCreateRequest>();
        CreateMap<PartyFormVM, PartyUpdateRequest>();
        CreateMap<PartyResponse, PartyListVM>();
        // FIX: mapping needed for Edit GET action
        CreateMap<PartyResponse, PartyFormVM>();
    }
}
