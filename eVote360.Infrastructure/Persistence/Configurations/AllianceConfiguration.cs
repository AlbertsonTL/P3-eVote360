using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Alliance;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class AllianceConfiguration : IEntityTypeConfiguration<Alliance>
{
    public void Configure(EntityTypeBuilder<Alliance> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasOne(a => a.PartyOrigen)
            .WithMany()
            .HasForeignKey(a => a.PartyOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.PartyDestino)
            .WithMany()
            .HasForeignKey(a => a.PartyDestinoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
