using MassTransit.Logging;

namespace Saga.PaymentService.Exceptions;

public class InvalidBankPaymentCode(string? message = "Invalid payment code") : Exception(message);