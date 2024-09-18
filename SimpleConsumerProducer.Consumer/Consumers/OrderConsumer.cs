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

        //await dbcontext.saveChangesAsync();
        await Task.Delay(1000);

        if (new Random().Next(0, 2) == 0)
        {
            _logger.LogInformation($"Random exception has been thrown! current time: {DateTime.Now}");
            // message redelivery on the consumer level test
            throw new Exception("Random exception");
        }
        
        _logger.LogInformation("Finished processing batch orders");
    }
}