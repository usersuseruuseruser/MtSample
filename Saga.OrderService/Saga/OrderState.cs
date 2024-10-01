using MassTransit;

namespace Saga.OrderService.Saga;

public class OrderState: SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; }
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; }
    public string Email { get; set; }
    public string WarehouseId { get; set; }
    public string BankPaymentCode { get; set; }
    public DateTime? OrderCreatedAt { get; set; }
    public DateTime? OrderCreationCompensatedAt { get; set; }
    public DateTime? StockReservedAt { get; set; }
    public DateTime? StockReservationCompensatedAt { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public DateTime? PaymentCompensatedAt { get; set; }
    public DateTime? NotificationSentAt { get; set; }
    public DateTime? NotificationCompensatedAt { get; set; }
    // generally it's like this public byte[] RowVersion { get; set; } but postgres has hidden row version column
    public uint RowVersion { get; set; }
}