using MassTransit;
using Saga.OrderService.Database;

namespace Saga.OrderService.Consumers.Definitions;

public class CreateOrderConsumerDefinition: ConsumerDefinition<CreateOrderConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateOrderConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}