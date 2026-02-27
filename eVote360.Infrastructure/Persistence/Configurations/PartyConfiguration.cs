using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Party;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class PartyConfiguration : IEntityTypeConfiguration<Party>
{
    public void Configure(EntityTypeBuilder<Party> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Descripcion).HasMaxLength(500);
        builder.Property(p => p.Siglas).IsRequired().HasMaxLength(10);
        builder.Property(p => p.LogoPath).IsRequired().HasMaxLength(500);

        builder.HasIndex(p => p.Siglas).IsUnique();
    }
}
