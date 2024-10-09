namespace Saga.OrderService.Dto;

public class OrderDto
{
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
    // куда отправить заказ
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    // с какого склада выбрал товар
    public Guid WarehouseId { get; set; }
    // код оплаты. в самом конце мы используем его чтобы получиь деньги от банка пользователя
    public string BankPaymentCode { get; set; } = null!;
}