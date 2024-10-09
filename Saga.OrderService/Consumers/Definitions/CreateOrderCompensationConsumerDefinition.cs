using MassTransit;
using Saga.OrderService.Database;

namespace Saga.OrderService.Consumers.Definitions;

public class CreateOrderCompensationConsumerDefinition: ConsumerDefinition<CreateOrderCompensationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CreateOrderCompensationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}