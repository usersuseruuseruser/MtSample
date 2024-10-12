using MassTransit;
using Saga.Contracts.PaymentRelated;
using Saga.PaymentService.Database;
using Saga.PaymentService.Database.Models;
using Saga.PaymentService.Exceptions;

namespace Saga.PaymentService.Consumers;

public class OrderPaymentConsumer: IConsumer<StartOrderPayment>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderPaymentCompensationConsumer> _logger;

    public OrderPaymentConsumer(AppDbContext dbContext, ILogger<OrderPaymentCompensationConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<StartOrderPayment> context)
    {
        _logger.LogInformation($"Payment for order {context.Message.OrderId} started");
        var message = context.Message;
        
        // check if bank really reserved the money for our transaction, we'll omit it here
        // also, in real system i think we should return overall sum from the delivery service calculated by
        // the delivery service itself, but we'll omit it here too as it's just educational project
        if (Random.Shared.Next(0,2) == 0)
        {
            throw new InvalidBankPaymentCode();
        }
        
        var paymentDateTime = DateTime.UtcNow;
        _dbContext.PaidOrders.Add(new PaidOrder()
        {
            OrderId = message.OrderId,
            BankPaymentCode = message.BankPaymentCode,
            PaymentDate = paymentDateTime
        });
        await context.Publish(new OrderPaymentSucceeded(message.OrderId, paymentDateTime));
        await _dbContext.SaveChangesAsync();
    }
}