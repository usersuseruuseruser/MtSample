namespace Saga.Contracts.DeliveryRelated;

public record PlanDelivery
{
    public Guid OrderId { get; init; }
    public Guid WarehouseId { get; init; }
    public Guid ItemId { get; init; }
    public int Quantity { get; init; }
    public string Address { get; init; } = null!;
}