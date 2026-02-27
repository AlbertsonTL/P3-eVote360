using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using eVote360.Domain.Entities.Assignments;

namespace eVote360.Infrastructure.Persistence.Configurations;

public class PartyAssignmentConfiguration : IEntityTypeConfiguration<PartyAssignments>
{
    public void Configure(EntityTypeBuilder<PartyAssignments> builder)
    {
        builder.HasKey(pa => pa.Id);

        builder.HasOne(pa => pa.Usuario)
            .WithMany()
            .HasForeignKey(pa => pa.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pa => pa.Party)
            .WithMany()
            .HasForeignKey(pa => pa.PartyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(pa => pa.UsuarioId).IsUnique();
    }
}
