using MassTransit;
using Saga.Contracts;
using Saga.OrderService.Consumers.Models;

namespace Saga.OrderService.Saga;

public class OrderStateMachine: MassTransitStateMachine<OrderState>
{
    public State SagaCreated { get; private set; } = null!;
    public State OrderCreated { get; private set; } = null!;
    public State OrderCreationFault { get; private set; } = null!;
    public Event<IStartOrderSaga> OrderSagaCreationEvent { get; private set; } = null!;
    public Event<IOrderCreated> OrderCreatedEvent { get; private set; } = null!;
    public Event<Fault<IOrderCreated>> OrderCreationFaultEvent { get; private set; } = null!;
    
    protected OrderStateMachine(State created)
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderSagaCreationEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCreatedEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCreationFaultEvent, x =>
            x.CorrelateById(context => context.Message.Message.OrderId));
        Initially(
            When(OrderSagaCreationEvent)
                .Then(context =>
                {
                    var saga = context.Saga;
                    var message = context.Message;
                    
                    // или .Then(context => context.Init(context.Message)) 
                    saga.ItemId = message.ItemId;
                    saga.ClientId = message.ClientId;
                    saga.Quantity = message.Quantity;
                    saga.Address = message.Address;
                    saga.Email = message.Email;
                    saga.WarehouseId = message.WarehouseId;
                    saga.BankPaymentCode = message.BankPaymentCode;
                })
                .Send(context => new CreateOrder
                {
                    OrderId = context.Saga.CorrelationId,
                    ItemId = context.Saga.ItemId,
                    ClientId = context.Saga.ClientId,
                    Quantity = context.Saga.Quantity
                })
                .TransitionTo(SagaCreated)
            );
        During(SagaCreated,
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderCreatedAt = context.Message.CreatedAt;
                })
                .TransitionTo(OrderCreated),
            When(OrderCreationFaultEvent)
                .TransitionTo(OrderCreationFault)
            );
    }
    
}