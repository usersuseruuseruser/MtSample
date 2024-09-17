using Contracts;
using MassTransit;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class OrderConsumer: IConsumer<Batch<CreateOrder>>
{
    private ILogger<OrderConsumer> _logger;

    public OrderConsumer(ILogger<OrderConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Batch<CreateOrder>> context)
    {
        _logger.LogInformation("Started processing batch orders");
        for (int i = 0; i < context.Message.Length; i++)
        {
            var data = context.Message[i];
            // dbcontext.orders.add(new CreateOrder{data.message.company, data.message.trees})
        }

        //await dbcontext.savechangesAsync();
        await Task.Delay(1000);
        _logger.LogInformation("Finished processing batch orders");
    }
}