using Microsoft.EntityFrameworkCore;
using eVote360.Domain.Entities;
using eVote360.Domain.Entities.Alliance;
using eVote360.Domain.Entities.Assignments;
using eVote360.Domain.Entities.Ballot;
using eVote360.Domain.Entities.Candidate;
using eVote360.Domain.Entities.Citizen;
using eVote360.Domain.Entities.Election;
using eVote360.Domain.Entities.Party;
using eVote360.Domain.Entities.Position;
using eVote360.Domain.Entities.Vote;

namespace eVote360.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Citizen> Citizens { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Party> Parties { get; set; }
    public DbSet<Candidate> Candidates { get; set; }
    public DbSet<Election> Elections { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<VoteItem> VoteItems { get; set; }
    public DbSet<PartyAssignments> PartyAssignments { get; set; }
    public DbSet<Alliance> Alliances { get; set; }
    public DbSet<Candidatura> Candidaturas { get; set; }
    public DbSet<ElectionBallot> ElectionBallots { get; set; }
    public DbSet<BallotOption> BallotOptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Los ValueObjects (EmailAddress, NationalId) se mapean via converters
        // en las EntityTypeConfiguration de cada entidad. No usar Ignore aqui.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
