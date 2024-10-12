using MassTransit;

namespace Saga.OrderService.Saga;

public class OrderState: SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid OrderId { get; set; }
    public string CurrentState { get; set; } = null!;
    public Guid ItemId { get; set; }
    public Guid ClientId { get; set; }
    public int Quantity { get; set; }
    public string Address { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Guid WarehouseId { get; set; }

    public string BankPaymentCode { get; set; } = null!;
    // is needed for faults like not enough stock, payment failed, etc.
    // not necessarily caused by the service itself
    public string? LastErrorMessage { get; set; }
    public DateTime? OrderCreatedAt { get; set; }
    public DateTime? OrderCreationCompensatedAt { get; set; }
    public DateTime? DeliveryPlannedAt { get; set; }
    public DateTime? DeliveryPlanningCompensatedAt { get; set; }
    public DateTime? PaymentCompletedAt { get; set; }
    public DateTime? PaymentCompensatedAt { get; set; }
    // generally it's like this public byte[] RowVersion { get; set; } but postgres has hidden row version column
    public uint RowVersion { get; set; }
}