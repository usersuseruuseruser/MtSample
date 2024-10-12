namespace Saga.Contracts.PaymentRelated;

public record StartOrderPayment(Guid OrderId, string BankPaymentCode);