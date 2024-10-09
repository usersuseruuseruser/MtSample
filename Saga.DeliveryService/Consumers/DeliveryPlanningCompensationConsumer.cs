using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Contracts.DeliveryRelated;
using Saga.DeliveryService.Database;

namespace Saga.DeliveryService.Consumers;

public class DeliveryPlanningCompensationConsumer: IConsumer<CompensateDeliveryPlanning>
{
    private readonly AppDbContext _dbContext;
    private ILogger<CompensateDeliveryPlanning> _logger;

    public DeliveryPlanningCompensationConsumer(AppDbContext dbContext, ILogger<CompensateDeliveryPlanning> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CompensateDeliveryPlanning> context)
    {
        _logger.LogInformation("Delivery planning compensation started for order {OrderId}", context.Message.OrderId);
        
        var message = context.Message;
        var delivery = await _dbContext.Deliveries.Where(d => d.OrderId == message.OrderId).FirstAsync();
        
        var stock = await _dbContext.Stocks.Where(s => s.WarehouseId == delivery.WarehouseId && s.ItemId == delivery.ItemId).FirstAsync();
        stock.Quantity += delivery.Quantity;
        _dbContext.Deliveries.Remove(delivery);

        await context.Publish<DeliveryPlanningCompensated>(new
        {
            OrderId = message.OrderId
        });
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"Delivery planning compensation for order {message.OrderId} completed");
    }
}