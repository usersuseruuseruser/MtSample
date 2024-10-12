using Saga.DeliveryService.Database.Models.Enums;

namespace Saga.DeliveryService.Database.Models;

public class Delivery
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public Guid WarehouseId { set; get; }
    public string Address { get; set; } = null!;
    public int Quantity { get; set; }
    public Status Status { get; set; }
}