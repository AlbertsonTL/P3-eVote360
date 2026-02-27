using Microsoft.EntityFrameworkCore;
using eVote360.Application.Abstractions.Repositories;
using eVote360.Domain.Entities.Alliance;
using eVote360.Domain.Enums;
using eVote360.Infrastructure.Persistence;
using eVote360.Infrastructure.Repositories.Base;

namespace eVote360.Infrastructure.Repositories;

public class AllianceRepository : GenericRepository<Alliance>, IAllianceRepository
{
    public AllianceRepository(AppDbContext context) : base(context) { }

    public async Task<IEnumerable<Alliance>> GetByPartyIdAsync(int partyId)
    {
        return await _dbSet
            .Include(a => a.PartyOrigen)
            .Include(a => a.PartyDestino)
            .Where(a => a.PartyOrigenId == partyId || a.PartyDestinoId == partyId)
            .OrderByDescending(a => a.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<Alliance?> GetPendingBetweenPartiesAsync(int partyAId, int partyBId)
    {
        return await _dbSet.FirstOrDefaultAsync(a =>
            a.Estado == AllianceStatus.EnEsperaDeRespuesta &&
            ((a.PartyOrigenId == partyAId && a.PartyDestinoId == partyBId) ||
             (a.PartyOrigenId == partyBId && a.PartyDestinoId == partyAId)));
    }

    public async Task<IEnumerable<Alliance>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(a => a.PartyOrigen)
            .Include(a => a.PartyDestino)
            .OrderByDescending(a => a.FechaSolicitud)
            .ToListAsync();
    }

    public async Task<bool> HasActiveAllianceAsync(int partyAId, int partyBId)
    {
        return await _dbSet.AnyAsync(a =>
            a.Estado == AllianceStatus.Aceptada &&
            ((a.PartyOrigenId == partyAId && a.PartyDestinoId == partyBId) ||
             (a.PartyOrigenId == partyBId && a.PartyDestinoId == partyAId)));
    }
}
