using MassTransit;
using SimpleConsumerProducer.Consumer.Consumers;
using SimpleConsumerProducer.Consumer.DbContext;

namespace SimpleConsumerProducer.Consumer.Definitions;

public class OrderWithEmailDefinition: ConsumerDefinition<OrderWithEmailConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderWithEmailConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
    }
}