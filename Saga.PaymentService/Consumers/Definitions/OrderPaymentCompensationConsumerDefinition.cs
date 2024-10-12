using MassTransit;
using Saga.PaymentService.Database;

namespace Saga.PaymentService.Consumers.Definitions;

public class OrderPaymentCompensationConsumerDefinition: ConsumerDefinition<OrderPaymentCompensationConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderPaymentCompensationConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}