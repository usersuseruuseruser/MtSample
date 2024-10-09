namespace Saga.Contracts.OrderRelated;

public interface ICreateOrder
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
}