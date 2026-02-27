using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eVote360.Domain.ValueObjects;

namespace eVote360.Infrastructure.Persistence.Converters;

public class NationalIdConverter : ValueConverter<NationalId, string>
{
    public NationalIdConverter() 
        : base(
            v => v.Value,
            v => new NationalId(v))
    {
    }
}
