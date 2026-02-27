using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using eVote360.Domain.ValueObjects;

namespace eVote360.Infrastructure.Persistence.Converters;

public class EmailAddressConverter : ValueConverter<EmailAddress, string>
{
    public EmailAddressConverter() 
        : base(
            v => v.Value,
            v => new EmailAddress(v))
    {
    }
}
