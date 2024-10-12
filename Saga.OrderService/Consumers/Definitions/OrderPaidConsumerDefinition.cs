using MassTransit;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Database;

namespace Saga.OrderService.Consumers.Definitions;

public class OrderPaidConsumerDefinition: ConsumerDefinition<OrderPaidConsumer>
{

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<OrderPaidConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, context);

        endpointConfigurator.PrefetchCount = 5;
        consumerConfigurator.Options<BatchOptions>(options =>
        {
            options.MessageLimit = 5;
            options.TimeLimit = TimeSpan.FromMilliseconds(500);
            options.ConcurrencyLimit = 3;
        });
    }
}