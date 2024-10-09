using Saga.Contracts.OrderRelated;

namespace Saga.OrderService.Consumers.Models;

public class OrderCreationCompensated: IOrderCreationCompensated
{
    public Guid OrderId { get; set; }
    public DateTime CompensatedAt { get; set; }
}