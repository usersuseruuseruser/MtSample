namespace Saga.Contracts.DeliveryRelated;

public interface IPlanDelivery
{
    public Guid OrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid ItemId { get; set; }
    public string Address { get; set; }
    public int Quantity { get; set; }
}