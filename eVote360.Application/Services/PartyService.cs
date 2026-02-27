using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Request;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Party;

namespace eVote360.Application.Services;

public class PartyService : IPartyService
{
    private readonly IPartyRepository _repository;
    private readonly ICandidateRepository _candidateRepository;
    private readonly IMapper _mapper;

    public PartyService(IPartyRepository repository, ICandidateRepository candidateRepository, IMapper mapper)
    {
        _repository = repository;
        _candidateRepository = candidateRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<PartyResponse>>> GetAllAsync()
    {
        var parties = await _repository.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<PartyResponse>>(parties);
        return Result<IEnumerable<PartyResponse>>.Success(dtos);
    }

    public async Task<Result<PartyResponse>> GetByIdAsync(int id)
    {
        var party = await _repository.GetByIdAsync(id);
        if (party == null)
            return Result<PartyResponse>.Failure("Partido no encontrado");

        var dto = _mapper.Map<PartyResponse>(party);
        return Result<PartyResponse>.Success(dto);
    }

    public async Task<Result<PartyResponse>> CreateAsync(PartyCreateRequest dto, string logoPath)
    {
        var exists = await _repository.GetBySiglasAsync(dto.Siglas);
        if (exists != null)
            return Result<PartyResponse>.Failure("Ya existe un partido con esas siglas");

        var party = _mapper.Map<Party>(dto);
        party.LogoPath = logoPath;
        party.IsActive = true;
        await _repository.AddAsync(party);

        var responseDto = _mapper.Map<PartyResponse>(party);
        return Result<PartyResponse>.Success(responseDto);
    }

    public async Task<Result<PartyResponse>> UpdateAsync(PartyUpdateRequest dto, string? logoPath)
    {
        var party = await _repository.GetByIdAsync(dto.Id);
        if (party == null)
            return Result<PartyResponse>.Failure("Partido no encontrado");

        var exists = await _repository.ExistsAsync(p => p.Siglas == dto.Siglas && p.Id != dto.Id);
        if (exists)
            return Result<PartyResponse>.Failure("Ya existe otro partido con esas siglas");

        party.Nombre = dto.Nombre;
        party.Descripcion = dto.Descripcion;
        party.Siglas = dto.Siglas;
        if (!string.IsNullOrEmpty(logoPath))
            party.LogoPath = logoPath;

        await _repository.UpdateAsync(party);
        var responseDto = _mapper.Map<PartyResponse>(party);
        return Result<PartyResponse>.Success(responseDto);
    }

    public async Task<Result> ToggleActiveAsync(int id)
    {
        var party = await _repository.GetByIdAsync(id);
        if (party == null)
            return Result.Failure("Partido no encontrado");

        bool wasActive = party.IsActive;
        party.IsActive = !party.IsActive;
        await _repository.UpdateAsync(party);

        // CASCADE: If deactivating party, deactivate all its candidates
        if (wasActive && !party.IsActive)
        {
            var candidates = await _candidateRepository.GetByPartyIdAsync(id);
            foreach (var candidate in candidates.Where(c => c.IsActive))
            {
                candidate.IsActive = false;
                await _candidateRepository.UpdateAsync(candidate);
            }
        }

        return Result.Success();
    }
}
