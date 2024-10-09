namespace Saga.Contracts.OrderRelated;

public interface IOrderCreated
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}