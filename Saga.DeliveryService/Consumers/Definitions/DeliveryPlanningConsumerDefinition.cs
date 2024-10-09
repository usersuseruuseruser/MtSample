using MassTransit;
using Saga.DeliveryService.Database;

namespace Saga.DeliveryService.Consumers.Definitions;

public class DeliveryPlanningConsumerDefinition: ConsumerDefinition<DeliveryPlanningConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeliveryPlanningConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}