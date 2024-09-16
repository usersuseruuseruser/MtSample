using Contracts;
using MassTransit;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class OrderConsumer: IConsumer<CreateOrder>
{
    private ILogger<OrderConsumer> _logger;

    public OrderConsumer(ILogger<OrderConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<CreateOrder> context)
    {
        // somehow process the data
        _logger.LogInformation($"Processed order {context.Message.Company} with {context.Message.Trees} trees");

        return Task.CompletedTask;
    }
}