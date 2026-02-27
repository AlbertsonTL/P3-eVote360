using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Assignments;

namespace eVote360.Application.Services;

public class PartyAssignmentService : IPartyAssignmentService
{
    private readonly IPartyAssignmentRepository _repository;
    private readonly IMapper _mapper;

    public PartyAssignmentService(IPartyAssignmentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PartyAssignmentResponseDto>>> GetAllAsync()
    {
        var assignments = await _repository.GetAllWithDetailsAsync();
        var dtos = _mapper.Map<IEnumerable<PartyAssignmentResponseDto>>(assignments);
        return Result<IEnumerable<PartyAssignmentResponseDto>>.Success(dtos);
    }

    public async Task<Result<PartyAssignmentResponseDto>> CreateAsync(PartyAssignmentCreateDto dto)
    {
        var exists = await _repository.GetByUsuarioIdAsync(dto.UsuarioId);
        if (exists != null)
            return Result<PartyAssignmentResponseDto>.Failure("Este dirigente ya está relacionado con otro partido político");

        var assignment = new PartyAssignments
        {
            UsuarioId = dto.UsuarioId,
            PartyId = dto.PartyId
        };

        await _repository.AddAsync(assignment);
        var responseDto = _mapper.Map<PartyAssignmentResponseDto>(assignment);
        return Result<PartyAssignmentResponseDto>.Success(responseDto);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var assignment = await _repository.GetByIdAsync(id);
        if (assignment == null)
            return Result.Failure("Asignación no encontrada");

        await _repository.DeleteAsync(assignment);
        return Result.Success();
    }
}
