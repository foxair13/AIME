namespace AI.SocialNetwork.Domain.ValueObjects;

// Оценка репутации (Value Object)
public class ReputationScore
{
    public decimal Value { get; }
    public string Level { get; }

    public ReputationScore(decimal value, string level)
    {
        Value = value;
        Level = level;
    }
}