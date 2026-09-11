namespace AI.SocialNetwork.Domain.Events;

public class ReputationUpdatedEvent
{
    public long UserId { get; set; }
    public long SkillId { get; set; }
    public decimal OldValue { get; set; }
    public decimal NewValue { get; set; }
}