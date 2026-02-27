using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Election;
using eVote360.Domain.Enums;

namespace eVote360.Application.Services;

public class ElectionService : IElectionService
{
    private readonly IElectionRepository _repository;
    private readonly IPositionRepository _positionRepository;
    private readonly IPartyRepository _partyRepository;
    private readonly ICandidateRepository _candidateRepository;

    public ElectionService(
        IElectionRepository repository,
        IPositionRepository positionRepository,
        IPartyRepository partyRepository,
        ICandidateRepository candidateRepository)
    {
        _repository = repository;
        _positionRepository = positionRepository;
        _partyRepository = partyRepository;
        _candidateRepository = candidateRepository;
    }

    public async Task<Result<Election>> GetActiveElectionAsync()
    {
        var election = await _repository.GetActiveElectionAsync();
        return election != null
            ? Result<Election>.Success(election)
            : Result<Election>.Failure("No hay elección activa");
    }

    public async Task<Result<IEnumerable<Election>>> GetAllAsync()
    {
        var elections = await _repository.GetAllAsync();
        return Result<IEnumerable<Election>>.Success(elections.OrderByDescending(e => e.FechaRealizacion));
    }

    public async Task<Result<Election>> GetByIdAsync(int id)
    {
        var election = await _repository.GetByIdAsync(id);
        return election != null
            ? Result<Election>.Success(election)
            : Result<Election>.Failure("Elección no encontrada");
    }

    public async Task<Result<List<string>>> ValidateCanCreateElectionAsync()
    {
        var errors = new List<string>();

        // 1. No debe existir ninguna elección activa
        var activeElection = await _repository.GetActiveElectionAsync();
        if (activeElection != null)
            errors.Add("Ya existe una elección activa en el sistema.");

        // 2. Debe existir al menos un puesto electivo activo
        var positions = await _positionRepository.GetAllAsync();
        var activePositions = positions.Where(p => p.IsActive).ToList();
        if (!activePositions.Any())
            errors.Add("Debe existir al menos un puesto electivo activo.");

        // 3. Deben existir al menos dos partidos políticos activos
        var parties = await _partyRepository.GetAllAsync();
        var activeParties = parties.Where(p => p.IsActive).ToList();
        if (activeParties.Count < 2)
            errors.Add($"Deben existir al menos dos partidos activos. Actualmente hay {activeParties.Count}.");

        // 4. Cada partido activo debe tener candidatos para TODOS los puestos activos
        if (errors.Count == 0 && activePositions.Any() && activeParties.Count >= 2)
        {
            foreach (var party in activeParties)
            {
                var candidates = await _candidateRepository.GetByPartyIdAsync(party.Id);
                var activeCandidates = candidates.Where(c => c.IsActive).ToList();

                foreach (var position in activePositions)
                {
                    var hasCandidate = activeCandidates.Any(c => c.PositionId == position.Id);
                    if (!hasCandidate)
                        errors.Add($"El partido '{party.Nombre}' no tiene candidato para el puesto '{position.Nombre}'.");
                }
            }
        }

        if (errors.Any())
            return Result<List<string>>.Failure(errors);

        return Result<List<string>>.Success(errors);
    }

    public async Task<Result<Election>> CreateElectionAsync(string nombre, DateTime fecha)
    {
        var validation = await ValidateCanCreateElectionAsync();
        if (!validation.IsSuccess)
            return Result<Election>.Failure(validation.Errors.Any() ? string.Join(" ", validation.Errors) : validation.Message);

        var election = new Election
        {
            Nombre = nombre,
            FechaRealizacion = fecha,
            Estado = ElectionState.EnProceso,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(election);
        return Result<Election>.Success(election);
    }

    public async Task<Result> FinalizeElectionAsync(int electionId)
    {
        var election = await _repository.GetByIdAsync(electionId);
        if (election == null)
            return Result.Failure("Elección no encontrada");

        election.Estado = ElectionState.Finalizada;
        await _repository.UpdateAsync(election);
        return Result.Success();
    }
}
