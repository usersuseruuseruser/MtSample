namespace Saga.OrderService.Database.Models;

public class Order
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; } = null!;
}