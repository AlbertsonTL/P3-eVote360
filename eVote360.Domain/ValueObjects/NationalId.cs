namespace eVote360.Domain.ValueObjects;

public record NationalId
{
    public string Value { get; init; }

    public NationalId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("National ID cannot be empty", nameof(value));

        Value = value;
    }

    public static implicit operator string(NationalId nationalId) => nationalId.Value;
    public static implicit operator NationalId(string value) => new(value);
}
