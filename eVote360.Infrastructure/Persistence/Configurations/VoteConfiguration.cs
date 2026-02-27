using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Vote;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class VoteConfiguration : IEntityTypeConfiguration<Vote>
{
    public void Configure(EntityTypeBuilder<Vote> builder)
    {
        builder.HasKey(v => v.Id);

        builder.HasOne(v => v.Citizen)
            .WithMany()
            .HasForeignKey(v => v.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(v => v.Election)
            .WithMany()
            .HasForeignKey(v => v.ElectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(v => v.VoteItems)
            .WithOne(vi => vi.Vote)
            .HasForeignKey(vi => vi.VoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
