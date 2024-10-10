using Saga.Contracts;
using Saga.Contracts.OrderRelated;

namespace Saga.OrderService.Consumers.Models;

public class OrderCreated: IOrderCreated
{
    public Guid OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
}