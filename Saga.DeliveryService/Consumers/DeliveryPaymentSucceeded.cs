using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Contracts.PaymentRelated;
using Saga.DeliveryService.Database;
using Saga.DeliveryService.Database.Models;
using Saga.DeliveryService.Database.Models.Enums;

namespace Saga.DeliveryService.Consumers;

public class DeliveryPaymentSucceeded: IConsumer<Batch<OrderPaymentSucceeded>>
{
    private readonly ILogger<DeliveryPaymentSucceeded> _logger;
    private readonly AppDbContext _dbContext;

    public DeliveryPaymentSucceeded(ILogger<DeliveryPaymentSucceeded> logger, AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<Batch<OrderPaymentSucceeded>> context)
    {
        var ids = context.Message.Select(mc => mc.Message.OrderId);

        // TODO: change to lookup table
        await _dbContext.Deliveries.ExecuteUpdateAsync(d =>
            d.SetProperty(del => del.Status, new Status() { Current = DeliveryStatus.Paid }));
    }
}