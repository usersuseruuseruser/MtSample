using MassTransit;
using Saga.Contracts;
using Saga.OrderService.Consumers.Models;
using Saga.OrderService.Database;
using Saga.OrderService.Enums;
using Saga.OrderService.Models;

namespace Saga.OrderService.Consumers;

public class CreateOrderConsumer: IConsumer<ICreateOrder>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateOrderConsumer> _logger;

    public CreateOrderConsumer(AppDbContext dbContext, ILogger<CreateOrderConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICreateOrder> context)
    {
        _logger.LogInformation("Creating a new order for item {ItemId} and client {ClientId}.", context.Message.ItemId, context.Message.ClientId);
        
        var order = new Order
        {
            Id = context.Message.OrderId,
            ItemId = context.Message.ItemId,
            CustomerId = context.Message.ClientId,
            Quantity = context.Message.Quantity,
            Status = new Status {OrderStatus = OrderStatus.Created},
            CreatedAt = DateTime.Now
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Order {OrderId} saved into database.", order.Id);
        // типо обработка заказа. в демо целях чтобы видеть разное состояние саги
        await Task.Delay(Random.Shared.Next(7899, 27599));
        
        await context.Publish<IOrderCreated>(new OrderCreated()
        {
            ItemId = order.ItemId,
            OrderId = order.Id,
            Quantity = order.Quantity,
            CreatedAt = order.CreatedAt
        });
    }
}