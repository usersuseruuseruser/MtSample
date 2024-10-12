using MassTransit;
using Saga.PaymentService.Database;

namespace Saga.PaymentService.Consumers.Definitions;

public class OrderPaymentConsumerDefinition: ConsumerDefinition<OrderPaymentConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderPaymentConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);
        
        endpointConfigurator.UseEntityFrameworkOutbox<AppDbContext>(context);
    }
}