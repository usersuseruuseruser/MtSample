namespace Saga.OrderService.Models;

public class Order
{
    public Guid Id { get; set; }
    // для упрощения предположим что один товар в каком-то количестве
    public Guid ItemId { get; set; }
    public Guid CustomerId { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public Status Status { get; set; } = null!;
}