using Saga.OrderService.Enums;

namespace Saga.OrderService.Models;

public class Status
{
    public Guid Id { get; set; }
    public OrderStatus OrderStatus { get; set; }
}