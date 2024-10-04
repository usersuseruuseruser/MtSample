namespace Saga.Contracts.OrderRelated;

public interface ICompensateOrderCreation
{
    public Guid OrderId { get; set; }
    
}