namespace Saga.Contracts.PaymentRelated;

public record OrderPaymentSucceeded(Guid OrderId, DateTime SucceededAt);