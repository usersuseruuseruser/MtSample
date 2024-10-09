namespace Saga.DeliveryService.Database.Models;

public class Warehouse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
}