using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Contracts.DeliveryRelated;
using Saga.DeliveryService.Database;
using Saga.DeliveryService.Database.Models;
using Saga.DeliveryService.Database.Models.Enums;
using Saga.DeliveryService.Exceptions;

namespace Saga.DeliveryService.Consumers;

public class DeliveryPlanningConsumer: IConsumer<PlanDelivery>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<DeliveryPlanningConsumer> _logger;

    public DeliveryPlanningConsumer(AppDbContext dbContext, ILogger<DeliveryPlanningConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PlanDelivery> context)
    {
        _logger.LogInformation("Planning delivery for order {OrderId}", context.Message.OrderId);
        var message = context.Message;
        
        // так как тут пример учебный, то и товаров в таблице нет, поэтому просто сделаем 50 на 50 что есть
        // var stock = await _dbContext.Stocks.Where(s => s.WarehouseId == message.WarehouseId && s.ItemId == message.ItemId).FirstAsync();
        // if (stock.Quantity < message.Quantity)
        // {
        //     throw new NotEnoughItemsInStockException(message.OrderId, message.ItemId, message.Quantity);
        // }
        // stock.Quantity -= message.Quantity;
        if (Random.Shared.Next(0,2) == 0)
        {
            throw new NotEnoughItemsInStockException(message.OrderId, message.ItemId, message.Quantity);
        }
        
        var delivery = new Delivery()
        {
            ItemId = message.ItemId,
            Quantity = message.Quantity,
            OrderId = context.Message.OrderId,
            WarehouseId = context.Message.WarehouseId,
            Status = new Status(){Current = DeliveryStatus.Created},
            Address = message.Address
        };
        _dbContext.Deliveries.Add(delivery);
        await context.Publish(new DeliveryPlanned(delivery.OrderId, DateTime.UtcNow));
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Delivery for order {OrderId} planned", context.Message.OrderId);
    }
}