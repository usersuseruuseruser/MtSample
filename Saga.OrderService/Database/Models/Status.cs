using Saga.OrderService.Database.Models.Enums;

namespace Saga.OrderService.Database.Models;

public class Status
{
    public Guid Id { get; set; }
    public OrderStatus Current { get; set; }
}