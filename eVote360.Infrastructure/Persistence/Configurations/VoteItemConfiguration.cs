using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Vote;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class VoteItemConfiguration : IEntityTypeConfiguration<VoteItem>
{
    public void Configure(EntityTypeBuilder<VoteItem> builder)
    {
        builder.HasKey(vi => vi.Id);

        builder.HasOne(vi => vi.Vote)
            .WithMany(v => v.VoteItems)
            .HasForeignKey(vi => vi.VoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(vi => vi.Position)
            .WithMany()
            .HasForeignKey(vi => vi.PositionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vi => vi.Candidate)
            .WithMany()
            .HasForeignKey(vi => vi.CandidateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(vi => vi.Party)
            .WithMany()
            .HasForeignKey(vi => vi.PartyId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
