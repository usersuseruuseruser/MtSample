using MassTransit;
using Saga.DeliveryService.Database;

namespace Saga.DeliveryService.Consumers.Definitions;

public class DeliveryPlanningCompensationConsumerDefinition: ConsumerDefinition<DeliveryPlanningCompensationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<DeliveryPlanningCompensationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}