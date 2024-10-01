using MassTransit;
using Saga.Contracts;
using Saga.OrderService.Database;
using Saga.OrderService.Models;

namespace Saga.OrderService.Consumers;

public class CreateOrderFaultConsumer: IConsumer<Fault<ICreateOrder>>
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<CreateOrderConsumer> _logger;

    public CreateOrderFaultConsumer(AppDbContext dbContext, ILogger<CreateOrderConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Fault<ICreateOrder>> context)
    {
        var order = await _dbContext.FindAsync<Order>(context.Message.Message.OrderId);
        
        // TODO: добавить outbox
        if (order != null)
        {
            _dbContext.Remove(order);
            await _dbContext.SaveChangesAsync();
            _logger.LogInformation("Order {OrderId} removed from database.", order.Id);
        }
        
        await context.RespondAsync<IOrderCreationCompensated>(new
        {
            OrderId = context.Message.Message.OrderId,
            CompensatedAt = DateTime.UtcNow
        });
    }
}