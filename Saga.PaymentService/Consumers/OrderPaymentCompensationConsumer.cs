using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Contracts.PaymentRelated;
using Saga.PaymentService.Database;

namespace Saga.PaymentService.Consumers;

public class OrderPaymentCompensationConsumer: IConsumer<CompensateOrderPayment>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<OrderPaymentCompensationConsumer> _logger;
    public OrderPaymentCompensationConsumer(AppDbContext dbContext, ILogger<OrderPaymentCompensationConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CompensateOrderPayment> context)
    {
        var message = context.Message;
        // doing something with the bank payment code. i didn't work in any bank so i don't know what to do here

        await _dbContext.PaidOrders.Where(p => p.OrderId == message.OrderId).ExecuteDeleteAsync();
        await context.Publish(new OrderPaymentCompensated(message.OrderId, DateTime.UtcNow));
    }
}