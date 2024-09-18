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
        IConsumerConfigurator<OrderConsumer> consumerConfigurator,
        IRegistrationContext registrationContext)
    {
        // will keep the lock on the message and contain it in the memory. not the best choice
        // because it just makes consumer wait without any work to do.
        /*consumerConfigurator.UseMessageRetry(r =>
        {
            r.Intervals(100, 200, 300, 500, 1000);
            r.Handle<ArgumentException>(r => r.ParamName == "Cookie cookie i'm a rookie!");
        });*/
        // we could just use redelivery instead of retry
        
        consumerConfigurator.Options<BatchOptions>(options =>
        {
            options.MessageLimit = 3;
            options.TimeLimit = TimeSpan.FromSeconds(10);
        });
    }
}