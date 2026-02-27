using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Election;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class ElectionConfiguration : IEntityTypeConfiguration<Election>
{
    public void Configure(EntityTypeBuilder<Election> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Estado).IsRequired();
    }
}
