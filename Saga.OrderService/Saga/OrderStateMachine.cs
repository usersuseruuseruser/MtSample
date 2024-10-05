using MassTransit;
using Saga.Contracts;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Consumers.Models;

namespace Saga.OrderService.Saga;

public class OrderStateMachine: MassTransitStateMachine<OrderState>
{
    public ILogger<OrderStateMachine> Logger { get; set; }
    public State SagaCreated { get; set; } = null!;
    public State OrderCreated { get; set; } = null!;
    public State OrderCreationFault { get; set; } = null!;
    public Event<IStartOrderSaga> OrderSagaCreationEvent { get; set; } = null!;
    public Event<IOrderCreated> OrderCreatedEvent { get; set; } = null!;
    public Event<Fault<ICreateOrder>> OrderCreationFaultEvent { get; set; } = null!;
    public Event<IOrderCreationCompensated> OrderCreationCompensationEvent { get; set; } = null!;


    public OrderStateMachine(ILogger<OrderStateMachine> logger)
    {
        Logger = logger;
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderSagaCreationEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCreatedEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCreationFaultEvent, x =>
            x.CorrelateById(context => context.Message.Message.OrderId));
        Event(() => OrderCreationCompensationEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Initially(
            When(OrderSagaCreationEvent)
                .Then(context =>
                {
                    var saga = context.Saga;
                    var message = context.Message;
                    
                    // или .Then(context => context.Init(context.Message)) 
                    saga.ItemId = message.ItemId;
                    saga.ClientId = message.ClientId;
                    saga.OrderId = message.OrderId;
                    saga.Quantity = message.Quantity;
                    saga.Address = message.Address;
                    saga.Email = message.Email;
                    saga.WarehouseId = message.WarehouseId;
                    saga.BankPaymentCode = message.BankPaymentCode;
                })
                .TransitionTo(SagaCreated)
                .Publish(context => new CreateOrder
                {
                    OrderId = context.Saga.CorrelationId,
                    ItemId = context.Saga.ItemId,
                    ClientId = context.Saga.ClientId,
                    Quantity = context.Saga.Quantity
                })
                .Then(context =>
                {
                    Logger.LogInformation("Order Saga {OrderId} created.", context.Saga.OrderId);
                })
            
        );
        During(SagaCreated,
            When(OrderCreatedEvent)
                .Then(context =>
                {
                    context.Saga.OrderCreatedAt = context.Message.CreatedAt;
                })
                .Then(context =>
                {
                    Logger.LogInformation("Order {OrderId} created.", context.Saga.OrderId);
                })
                .TransitionTo(OrderCreated),
                
                When(OrderCreationFaultEvent)
                .Publish(context => new CompensateOrderCreation()
                {
                    OrderId = context.Saga.OrderId
                })
                .Then(context =>
                {
                    Logger.LogInformation("Order {OrderId} creation fault, compensation event published.",
                        context.Saga.OrderId);
                })
                .TransitionTo(OrderCreationFault)
            );
        During(OrderCreationFault,
            When(OrderCreationCompensationEvent)
                .Then(context =>
                {
                    context.Saga.OrderCreationCompensatedAt = context.Message.CompensatedAt;
                })
                .Then(context =>
                {
                    Logger.LogInformation("Order {OrderId} creation fault compensated.", context.Saga.OrderId);
                })
                .Finalize()
            );
    }
    
}