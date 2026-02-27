using AutoMapper;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.ViewModels.Position;
using eVote360.Domain.Entities.Position;

namespace eVote360.Application.Profiles;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<Position, PositionResponse>();
        CreateMap<PositionCreateRequest, Position>();
        CreateMap<PositionFormVM, PositionCreateRequest>();
        CreateMap<PositionFormVM, PositionUpdateRequest>();
        CreateMap<PositionResponse, PositionListVM>();
        CreateMap<PositionResponse, PositionFormVM>();
    }
}
