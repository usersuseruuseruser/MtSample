using MassTransit;
using MassTransit.RabbitMqTransport.Configuration;
using SimpleConsumerProducer.Consumer.Consumers;

namespace SimpleConsumerProducer.Consumer.Definitions;

public class OrderConsumerDefinition: ConsumerDefinition<OrderConsumer>
{
    public OrderConsumerDefinition()
    {
        // EndpointName = "123123";
        // ConcurrentMessageLimit = 123;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, 
        IConsumerConfigurator<OrderConsumer> consumerConfigurator)
    {
        consumerConfigurator.Options<BatchOptions>(options =>
        {
            options.MessageLimit = 3;
            options.TimeLimit = TimeSpan.FromSeconds(10);
        });
    }
}