using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class CandidaturaConfiguration : IEntityTypeConfiguration<Candidatura>
{
    public void Configure(EntityTypeBuilder<Candidatura> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Candidate)
            .WithMany()
            .HasForeignKey(c => c.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Position)
            .WithMany()
            .HasForeignKey(c => c.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Party)
            .WithMany()
            .HasForeignKey(c => c.PartyId)
            .OnDelete(DeleteBehavior.Restrict);

        // A candidate can only be assigned once per party per position
        builder.HasIndex(c => new { c.CandidateId, c.PartyId }).IsUnique();
    }
}
