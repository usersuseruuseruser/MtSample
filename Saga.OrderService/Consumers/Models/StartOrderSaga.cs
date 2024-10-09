using Saga.Contracts;

namespace Saga.OrderService.Consumers.Models;

public class StartOrderSaga: IStartOrderSaga
{
    public Guid OrderId { get; set; }
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid WarehouseId { get; set; }
    public string BankPaymentCode { get; set; } = null!;
}