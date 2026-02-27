namespace eVote360.Domain.ValueObjects;

public record EmailAddress
{
    public string Value { get; init; }

    public EmailAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Email cannot be empty", nameof(value));

        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email format", nameof(value));

        Value = value;
    }

    public static implicit operator string(EmailAddress email) => email.Value;
    public static implicit operator EmailAddress(string value) => new(value);
}
