using MassTransit;
using Saga.OrderService.Database;

namespace Saga.OrderService.Saga;

public class OrderSagaDefinition: SagaDefinition<OrderState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<OrderState> sagaConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseMessageRetry(r => r.Interval(5, 100));
        endpointConfigurator.UseEntityFrameworkOutbox<OrderSagaDbContext>(context);
    }
}