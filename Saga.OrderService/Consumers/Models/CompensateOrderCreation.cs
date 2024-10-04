using Saga.Contracts;
using Saga.Contracts.OrderRelated;

namespace Saga.OrderService.Consumers.Models;

public class CompensateOrderCreation: ICompensateOrderCreation
{
    public Guid OrderId { get; set; }
}