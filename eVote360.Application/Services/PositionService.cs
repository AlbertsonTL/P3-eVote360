using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Position;

namespace eVote360.Application.Services;

public class PositionService : IPositionService
{
    private readonly IPositionRepository _repository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IMapper _mapper;

    public PositionService(IPositionRepository repository, ICandidateRepository candidateRepository, IMapper mapper)
    {
        _repository = repository;
        _candidateRepository = candidateRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PositionResponse>>> GetAllAsync()
    {
        var positions = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<PositionResponse>>(positions);
        return Result<IEnumerable<PositionResponse>>.Success(dtos);
    }

    public async Task<Result<PositionResponse>> GetByIdAsync(int id)
    {
        var position = await _repository.GetByIdAsync(id);
        if (position == null)
            return Result<PositionResponse>.Failure("Puesto no encontrado");

        var dto = _mapper.Map<PositionResponse>(position);
        return Result<PositionResponse>.Success(dto);
    }

    public async Task<Result<PositionResponse>> CreateAsync(PositionCreateRequest dto)
    {
        var position = _mapper.Map<Position>(dto);
        position.IsActive = true;
        await _repository.AddAsync(position);

        var responseDto = _mapper.Map<PositionResponse>(position);
        return Result<PositionResponse>.Success(responseDto);
    }

    public async Task<Result<PositionResponse>> UpdateAsync(PositionUpdateRequest dto)
    {
        var position = await _repository.GetByIdAsync(dto.Id);
        if (position == null)
            return Result<PositionResponse>.Failure("Puesto no encontrado");

        position.Nombre = dto.Nombre;
        position.Descripcion = dto.Descripcion;

        await _repository.UpdateAsync(position);
        var responseDto = _mapper.Map<PositionResponse>(position);
        return Result<PositionResponse>.Success(responseDto);
    }

    public async Task<Result> ToggleActiveAsync(int id)
    {
        var position = await _repository.GetByIdAsync(id);
        if (position == null)
            return Result.Failure("Puesto no encontrado");

        bool wasActive = position.IsActive;
        position.IsActive = !position.IsActive;
        await _repository.UpdateAsync(position);

        // CASCADE: If deactivating, deactivate all candidates assigned to this position
        if (wasActive && !position.IsActive)
        {
            var candidates = await _candidateRepository.GetByPositionIdAsync(id);
            foreach (var candidate in candidates.Where(c => c.IsActive))
            {
                candidate.IsActive = false;
                await _candidateRepository.UpdateAsync(candidate);
            }
        }

        return Result.Success();
    }
}
