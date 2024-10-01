namespace Saga.Contracts;

public interface IOrderCreated
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalCount { get; set; }
    public DateTime CreatedAt { get; set; }
}