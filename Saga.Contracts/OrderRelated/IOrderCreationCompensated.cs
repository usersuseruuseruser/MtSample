namespace Saga.Contracts.OrderRelated;

public interface IOrderCreationCompensated
{
    public Guid OrderId { get; }
    public DateTime CompensatedAt { get; }
}