using MassTransit;
using Saga.Contracts;
using Saga.Contracts.OrderRelated;
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

        // типо чтото делаем
        await Task.Delay(Random.Shared.Next(2000, 5000));
        
        var order = new Order
        {
            Id = context.Message.OrderId,
            ItemId = context.Message.ItemId,
            CustomerId = context.Message.ClientId,
            Quantity = context.Message.Quantity,
            Status = new Status {OrderStatus = OrderStatus.Created},
            CreatedAt = DateTime.Now.ToUniversalTime()
        };
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        
        // специально выбрасываю после сохранения в бд, чтобы показать что сага сможет обработать эту ситуацию
        if (Random.Shared.Next(0,2) != 0)
        {
            throw new Exception("Random exception during order creation");
        }
        
        _logger.LogInformation("Order {OrderId} saved into database.", order.Id);
       
        
        await context.Publish<IOrderCreated>(new OrderCreated()
        {
            ItemId = order.ItemId,
            OrderId = order.Id,
            Quantity = order.Quantity,
            CreatedAt = order.CreatedAt
        });
    }
}