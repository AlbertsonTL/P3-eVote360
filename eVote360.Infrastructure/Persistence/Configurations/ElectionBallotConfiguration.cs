using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Ballot;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class ElectionBallotConfiguration : IEntityTypeConfiguration<ElectionBallot>
{
    public void Configure(EntityTypeBuilder<ElectionBallot> builder)
    {
        builder.HasKey(eb => eb.Id);

        builder.HasOne(eb => eb.Election)
            .WithMany()
            .HasForeignKey(eb => eb.ElectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(eb => eb.Position)
            .WithMany()
            .HasForeignKey(eb => eb.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
