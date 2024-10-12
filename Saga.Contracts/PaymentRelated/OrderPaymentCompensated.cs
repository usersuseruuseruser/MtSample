namespace Saga.Contracts.PaymentRelated;

public record OrderPaymentCompensated(Guid OrderId, DateTime CompensatedAt);