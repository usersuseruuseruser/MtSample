namespace Saga.Contracts;

public interface IStartOrderSaga
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; }
    public string Email { get; set; } 
    public string WarehouseId { get; set; } 
    public string BankPaymentCode { get; set; } 
}