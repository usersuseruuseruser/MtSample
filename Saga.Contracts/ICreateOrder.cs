namespace Saga.Contracts;

public interface ICreateOrder
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
}