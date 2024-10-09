namespace Saga.Contracts.OrderRelated;

public interface IOrderCreationCompensated
{
    public Guid OrderId { get; set; }
    public DateTime CompensatedAt { get; set; }
}