using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Candidate;

namespace eVote360.Application.Services;

public class CandidateService : ICandidateService
{
    private readonly ICandidateRepository _repository;
    private readonly IAllianceRepository _allianceRepository;
    private readonly IMapper _mapper;

    public CandidateService(ICandidateRepository repository, IAllianceRepository allianceRepository, IMapper mapper)
    {
        _repository = repository;
        _allianceRepository = allianceRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<CandidateResponseDto>>> GetAllAsync()
    {
        var candidates = await _repository.GetAllWithDetailsAsync();
        var dtos = _mapper.Map<IEnumerable<CandidateResponseDto>>(candidates);
        return Result<IEnumerable<CandidateResponseDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<CandidateResponseDto>>> GetByPartyIdAsync(int partyId)
    {
        var candidates = await _repository.GetByPartyIdAsync(partyId);
        var dtos = _mapper.Map<IEnumerable<CandidateResponseDto>>(candidates);
        return Result<IEnumerable<CandidateResponseDto>>.Success(dtos);
    }

    public async Task<Result<CandidateResponseDto>> GetByIdAsync(int id)
    {
        var candidates = await _repository.GetAllWithDetailsAsync();
        var candidate = candidates.FirstOrDefault(c => c.Id == id);
        if (candidate == null)
            return Result<CandidateResponseDto>.Failure("Candidato no encontrado");
        var dto = _mapper.Map<CandidateResponseDto>(candidate);
        return Result<CandidateResponseDto>.Success(dto);
    }

    public async Task<Result<CandidateResponseDto>> CreateAsync(CandidateCreateRequestDto dto, string fotoPath)
    {
        var candidate = new Candidate
        {
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            PartyId = dto.PartyId,
            FotoPath = fotoPath,
            IsActive = true
        };

        await _repository.AddAsync(candidate);
        var responseDto = _mapper.Map<CandidateResponseDto>(candidate);
        return Result<CandidateResponseDto>.Success(responseDto);
    }

    public async Task<Result<CandidateResponseDto>> UpdateAsync(CandidateUpdateRequestDto dto, string? fotoPath)
    {
        var candidate = await _repository.GetByIdAsync(dto.Id);
        if (candidate == null)
            return Result<CandidateResponseDto>.Failure("Candidato no encontrado");

        candidate.Nombre = dto.Nombre;
        candidate.Apellido = dto.Apellido;
        if (!string.IsNullOrEmpty(fotoPath))
            candidate.FotoPath = fotoPath;

        await _repository.UpdateAsync(candidate);
        var responseDto = _mapper.Map<CandidateResponseDto>(candidate);
        return Result<CandidateResponseDto>.Success(responseDto);
    }

    public async Task<Result> ToggleActiveAsync(int id)
    {
        var candidate = await _repository.GetByIdAsync(id);
        if (candidate == null)
            return Result.Failure("Candidato no encontrado");

        candidate.IsActive = !candidate.IsActive;
        await _repository.UpdateAsync(candidate);
        return Result.Success();
    }

    public async Task<Result> AssignPositionAsync(int candidateId, int positionId, int partyId)
    {
        var candidate = await _repository.GetByIdAsync(candidateId);
        if (candidate == null)
            return Result.Failure("Candidato no encontrado");

        if (candidate.PartyId != partyId)
            return Result.Failure("El candidato no pertenece a este partido");

        // Rule: A candidate cannot run for more than one position within the same party
        // But can run for the SAME position in allied parties
        // Check if candidate already has a different position assigned in this party
        var partyCandidates = await _repository.GetByPartyIdAsync(partyId);
        var existingForPosition = partyCandidates.FirstOrDefault(c =>
            c.Id == candidateId && c.PositionId.HasValue && c.PositionId != positionId);

        if (existingForPosition != null)
            return Result.Failure("Este candidato ya está postulado para otro puesto en este partido");

        candidate.PositionId = positionId;
        await _repository.UpdateAsync(candidate);
        return Result.Success();
    }
}
