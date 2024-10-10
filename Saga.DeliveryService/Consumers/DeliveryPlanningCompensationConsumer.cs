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
        var delivery = await _dbContext.Deliveries.Where(d => d.OrderId == message.OrderId).FirstOrDefaultAsync();
        if (delivery != null)
        {
            _dbContext.Deliveries.Remove(delivery);
            // так как пример учебный, то товаров у нас нет в стоке... логика как ниже должна быть
            // var stock = await _dbContext.Stocks.Where(s => s.WarehouseId == delivery.WarehouseId && s.ItemId == delivery.ItemId).FirstAsync();
            // if (stock != null)
            // stock.Quantity += delivery.Quantity;
            await _dbContext.SaveChangesAsync();
        }
        
        await context.Publish(new DeliveryPlanningCompensated(message.OrderId, DateTime.UtcNow));
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation($"Delivery planning compensation for order {message.OrderId} completed");
    }
}