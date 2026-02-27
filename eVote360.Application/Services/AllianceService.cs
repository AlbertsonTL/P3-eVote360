using AutoMapper;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Application.Abstractions.Services;
using eVote360.Application.DTOs.Response;
using eVote360.Application.Results;
using eVote360.Domain.Entities.Alliance;
using eVote360.Domain.Enums;

namespace eVote360.Application.Services;

public class AllianceService : IAllianceService
{
    private readonly IAllianceRepository _repository;
    private readonly IMapper _mapper;

    public AllianceService(IAllianceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<AllianceResponseDto>>> GetByPartyIdAsync(int partyId)
    {
        var alliances = await _repository.GetByPartyIdAsync(partyId);
        var dtos = alliances.Select(a => new AllianceResponseDto
        {
            Id = a.Id,
            PartyOrigenId = a.PartyOrigenId,
            PartyOrigenNombre = a.PartyOrigen?.Nombre ?? "",
            PartyOrigenSiglas = a.PartyOrigen?.Siglas ?? "",
            PartyDestinoId = a.PartyDestinoId,
            PartyDestinoNombre = a.PartyDestino?.Nombre ?? "",
            PartyDestinoSiglas = a.PartyDestino?.Siglas ?? "",
            Estado = a.Estado.ToString(),
            FechaSolicitud = a.FechaSolicitud,
            FechaRespuesta = a.FechaRespuesta
        });
        return Result<IEnumerable<AllianceResponseDto>>.Success(dtos);
    }

    public async Task<Result<AllianceResponseDto>> CreateRequestAsync(int partyOrigenId, int partyDestinoId)
    {
        if (partyOrigenId == partyDestinoId)
            return Result<AllianceResponseDto>.Failure("No puede solicitar alianza con su propio partido");

        // Check if there's already a pending request between these parties
        var pending = await _repository.GetPendingBetweenPartiesAsync(partyOrigenId, partyDestinoId);
        if (pending != null)
            return Result<AllianceResponseDto>.Failure("Ya existe una solicitud de alianza pendiente entre estos partidos");

        // Check if they already have an active alliance
        var hasActive = await _repository.HasActiveAllianceAsync(partyOrigenId, partyDestinoId);
        if (hasActive)
            return Result<AllianceResponseDto>.Failure("Ya existe una alianza activa entre estos partidos");

        var alliance = new Alliance
        {
            PartyOrigenId = partyOrigenId,
            PartyDestinoId = partyDestinoId,
            Estado = AllianceStatus.EnEsperaDeRespuesta,
            FechaSolicitud = DateTime.UtcNow
        };

        await _repository.AddAsync(alliance);
        return Result<AllianceResponseDto>.Success(new AllianceResponseDto
        {
            Id = alliance.Id,
            PartyOrigenId = alliance.PartyOrigenId,
            PartyDestinoId = alliance.PartyDestinoId,
            Estado = alliance.Estado.ToString(),
            FechaSolicitud = alliance.FechaSolicitud
        });
    }

    public async Task<Result> AcceptAsync(int allianceId, int partyDestinoId)
    {
        var alliance = await _repository.GetByIdAsync(allianceId);
        if (alliance == null)
            return Result.Failure("Solicitud de alianza no encontrada");

        if (alliance.PartyDestinoId != partyDestinoId)
            return Result.Failure("No tiene permisos para responder esta solicitud");

        if (alliance.Estado != AllianceStatus.EnEsperaDeRespuesta)
            return Result.Failure("Esta solicitud ya fue respondida");

        alliance.Estado = AllianceStatus.Aceptada;
        alliance.FechaRespuesta = DateTime.UtcNow;
        await _repository.UpdateAsync(alliance);
        return Result.Success();
    }

    public async Task<Result> RejectAsync(int allianceId, int partyDestinoId)
    {
        var alliance = await _repository.GetByIdAsync(allianceId);
        if (alliance == null)
            return Result.Failure("Solicitud de alianza no encontrada");

        if (alliance.PartyDestinoId != partyDestinoId)
            return Result.Failure("No tiene permisos para responder esta solicitud");

        if (alliance.Estado != AllianceStatus.EnEsperaDeRespuesta)
            return Result.Failure("Esta solicitud ya fue respondida");

        alliance.Estado = AllianceStatus.Rechazada;
        alliance.FechaRespuesta = DateTime.UtcNow;
        await _repository.UpdateAsync(alliance);
        return Result.Success();
    }

    public async Task<bool> HasActiveAllianceAsync(int partyAId, int partyBId)
    {
        return await _repository.HasActiveAllianceAsync(partyAId, partyBId);
    }

    public async Task<IEnumerable<int>> GetAlliedPartyIdsAsync(int partyId)
    {
        var alliances = await _repository.GetByPartyIdAsync(partyId);
        return alliances
            .Where(a => a.Estado == AllianceStatus.Aceptada)
            .Select(a => a.PartyOrigenId == partyId ? a.PartyDestinoId : a.PartyOrigenId)
            .Distinct()
            .ToList();
    }
}