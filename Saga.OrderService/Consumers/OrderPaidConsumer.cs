using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Contracts.OrderRelated;
using Saga.Contracts.PaymentRelated;
using Saga.OrderService.Database;
using Saga.OrderService.Database.Models.Enums;

namespace Saga.OrderService.Consumers;

public class OrderPaidConsumer: IConsumer<Batch<OrderPaymentSucceeded>>
{
    private readonly ILogger<OrderPaidConsumer> _logger;
    private readonly AppDbContext _dbContext;

    public OrderPaidConsumer(ILogger<OrderPaidConsumer> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<Batch<OrderPaymentSucceeded>> context)
    {
        var ids = context.Message.Select(mc => mc.Message.OrderId);

        // TODO: change to lookup table
        await _dbContext.Orders.Where(o => ids.Contains(o.Id))
            .ExecuteUpdateAsync(x => x.SetProperty(o => o.Status.Current, OrderStatus.Paid));
    }
}