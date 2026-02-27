using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Ballot;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class BallotOptionConfiguration : IEntityTypeConfiguration<BallotOption>
{
    public void Configure(EntityTypeBuilder<BallotOption> builder)
    {
        builder.HasKey(bo => bo.Id);

        builder.HasOne(bo => bo.ElectionBallot)
            .WithMany()
            .HasForeignKey(bo => bo.ElectionBallotId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bo => bo.Candidate)
            .WithMany()
            .HasForeignKey(bo => bo.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bo => bo.Party)
            .WithMany()
            .HasForeignKey(bo => bo.PartyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
