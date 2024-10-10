using MassTransit;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Consumers.Models;
using Saga.OrderService.Database;
using Saga.OrderService.Database.Models;

namespace Saga.OrderService.Consumers;

public class CreateOrderCompensationConsumer: IConsumer<ICompensateOrderCreation>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateOrderCompensationConsumer> _logger;

    public CreateOrderCompensationConsumer(AppDbContext dbContext, ILogger<CreateOrderCompensationConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICompensateOrderCreation> context)
    {
        _logger.LogInformation("Compensating order creation for order {OrderId}.", context.Message.OrderId);
        var order = await _dbContext.FindAsync<Order>(context.Message.OrderId);
        
        if (order != null)
        {
            _dbContext.Remove(order);
            await _dbContext.SaveChangesAsync();
        }
        await context.Publish((IOrderCreationCompensated) new OrderCreationCompensated()
        {
            OrderId = context.Message.OrderId,
            CompensatedAt = DateTime.Now.ToUniversalTime()
        });
        _logger.LogInformation("Order {OrderId} removed from database.", context.Message.OrderId);
    }
}