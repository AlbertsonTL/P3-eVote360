using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Citizen;
using eVote360.Infrastructure.Persistence.Converters;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
{
    public void Configure(EntityTypeBuilder<Citizen> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Apellido).IsRequired().HasMaxLength(100);
        
        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(new EmailAddressConverter());

        builder.Property(c => c.NumeroDocumento)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(new NationalIdConverter());

        builder.HasIndex(c => c.NumeroDocumento).IsUnique();
    }
}
