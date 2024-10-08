using Saga.Contracts;
using Saga.Contracts.OrderRelated;

namespace Saga.OrderService.Consumers.Models;

public class CreateOrder: ICreateOrder
{
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
}