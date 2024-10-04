using MassTransit;
using Saga.Contracts;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Database;
using Saga.OrderService.Models;

namespace Saga.OrderService.Consumers;

public class CreateOrderCompensationConsumer: IConsumer<ICompensateOrderCreation>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateOrderConsumer> _logger;

    public CreateOrderCompensationConsumer(AppDbContext dbContext, ILogger<CreateOrderConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICompensateOrderCreation> context)
    {
        var order = await _dbContext.FindAsync<Order>(context.Message.OrderId);
        
        // TODO: добавить outbox
        if (order != null)
        {
            _dbContext.Remove(order);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Order {OrderId} removed from database.", order.Id);
        }
        
        await context.Publish<IOrderCreationCompensated>(new
        {
            OrderId = context.Message.OrderId,
            CompensatedAt = DateTime.UtcNow
        });
    }
}