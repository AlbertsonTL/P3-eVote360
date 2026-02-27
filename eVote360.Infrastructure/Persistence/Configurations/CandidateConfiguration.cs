using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Candidate;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class CandidateConfiguration : IEntityTypeConfiguration<Candidate>
{
    public void Configure(EntityTypeBuilder<Candidate> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);
        builder.Property(c => c.FotoPath).IsRequired().HasMaxLength(500);

        builder.HasOne(c => c.Party)
            .WithMany()
            .HasForeignKey(c => c.PartyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Position)
            .WithMany()
            .HasForeignKey(c => c.PositionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
