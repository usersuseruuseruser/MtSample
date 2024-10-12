using MassTransit;
using Saga.Contracts;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Consumers.Models;
using Saga.OrderService.Database;
using Saga.OrderService.Database.Models;
using Saga.OrderService.Database.Models.Enums;

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
        _logger.LogInformation("Creating a new order for client {ClientId} with order id {OrderId}",  context.Message.ClientId, context.Message.OrderId);
        
        var order = new Order
        {
            Id = context.Message.OrderId,
            CustomerId = context.Message.ClientId,
            Status = new Status {Current = OrderStatus.Created},
            CreatedAt = DateTime.Now.ToUniversalTime()
        };
        _dbContext.Orders.Add(order);
        
        await context.Publish((IOrderCreated) new OrderCreated()
        {
            OrderId = order.Id,
            CreatedAt = order.CreatedAt
        });

        if (Random.Shared.Next(0,2) == 0)
        {
            throw new Exception("Random exception during order creation");
        }
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Order {OrderId} saved into database.", order.Id);
    }
}