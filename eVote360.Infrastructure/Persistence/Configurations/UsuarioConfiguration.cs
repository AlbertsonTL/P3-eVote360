using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities;
using eVote360.Infrastructure.Persistence.Converters;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Apellido).IsRequired().HasMaxLength(100);
        builder.Property(u => u.NombreUsuario).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Rol).IsRequired().HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasConversion(new EmailAddressConverter());

        builder.HasIndex(u => u.NombreUsuario).IsUnique();
    }
}
