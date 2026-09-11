namespace AI.SocialNetwork.Domain.Events;

public record DealCompletedEvent(long DealId, long ClientId, long ContractorId, decimal Amount)
{
    public static DealCompletedEvent From(Entities.Deal deal)
        => new(deal.Id, deal.ClientId, deal.ContractorId, deal.Amount);
}