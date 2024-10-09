namespace Saga.DeliveryService.Database.Models;

public class Stock
{
    public Guid Id { get; set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public Guid WarehouseId { get; set; }
}