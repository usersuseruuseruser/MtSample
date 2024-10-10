using MassTransit;
using Saga.Contracts;
using Saga.Contracts.DeliveryRelated;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Consumers.Models;

namespace Saga.OrderService.Saga;

public class OrderStateMachine: MassTransitStateMachine<OrderState>
{
    public ILogger<OrderStateMachine> Logger { get; set; }
    public State SagaCreated { get; set; } = null!;
    public State OrderCreated { get; set; } = null!;
    public State OrderCreationFault { get; set; } = null!;
    public State DeliveryPlanned { get; set; } = null!;
    public State DeliveryPlanningFault { get; set; } = null!;
    public Event<IStartOrderSaga> OrderSagaCreationEvent { get; set; } = null!;
    
    public Event<IOrderCreated> OrderCreatedEvent { get; set; } = null!;
    public Event<Fault<ICreateOrder>> OrderCreationFaultEvent { get; set; } = null!;
    public Event<IOrderCreationCompensated> OrderCreationCompensationEvent { get; set; } = null!;
    
    public Event<DeliveryPlanned> DeliveryPlannedEvent { get; set; } = null!;
    public Event<Fault<PlanDelivery>> DeliveryPlanningFaultEvent { get; set; } = null!;
    public Event<DeliveryPlanningCompensated> DeliveryPlanningCompensatedEvent { get; set; } = null!;
    

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
        Event(() => DeliveryPlannedEvent, x =>
            x.CorrelateById(context => context.Message.OrderId));
        Event(() => DeliveryPlanningFaultEvent, x =>
            x.CorrelateById(context => context.Message.Message.OrderId));
        Event(() => DeliveryPlanningCompensatedEvent, x =>
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
                // в общем, зря я тут использовал интерфейсы. transactional outbox в mass transit почему-то не работает с наследниками
                // а работает только с конкретными типами: никакие наследники не биндятся в обменники rabbitmq
                // то есть если убрать кофнигурацию transactional outbox, то код ниже будет работать без каста
                // но т.к. исправлять долго, то я просто кастую и в дальнейшем буду использовать рекорды
                .Publish(context => (ICreateOrder) new CreateOrder
                {
                    OrderId = context.Saga.CorrelationId,
                    ClientId = context.Saga.ClientId,
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
                    Logger.LogInformation("Order {OrderId} created.", context.Saga.OrderId);
                })
                .Publish(context => new PlanDelivery()
                {
                    OrderId = context.Saga.OrderId,
                    WarehouseId = context.Saga.WarehouseId,
                    ItemId = context.Saga.ItemId,
                    Quantity = context.Saga.Quantity,
                    Address = context.Saga.Address
                })
                .TransitionTo(OrderCreated),
                
                When(OrderCreationFaultEvent)
                .Publish(context => (ICompensateOrderCreation) new CompensateOrderCreation()
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
                    Logger.LogInformation("Order {OrderId} creation fault compensated.", context.Saga.OrderId);
                })
                .Finalize()
            );
        During(OrderCreated,
            When(DeliveryPlannedEvent)
                .Then(context =>
                {
                    context.Saga.DeliveryPlannedAt = context.Message.PlannedAt;
                    Logger.LogInformation("Delivery planned for order {OrderId}.", context.Saga.OrderId);
                })
                .TransitionTo(DeliveryPlanned),
            
            When(DeliveryPlanningFaultEvent)
                .Publish(context => new CompensateDeliveryPlanning(context.Saga.OrderId))
                .Then(context =>
                {
                    Logger.LogInformation("Delivery planning fault for order {OrderId}, compensation event published.",
                        context.Saga.OrderId);
                })
                .TransitionTo(DeliveryPlanningFault)
            );
        During(DeliveryPlanningFault,
            When(DeliveryPlanningCompensatedEvent)
                .Then(context =>
                {
                    context.Saga.DeliveryPlanningCompensatedAt = context.Message.CompensatedAt;
                    Logger.LogInformation("Delivery planning fault compensated for order {OrderId}.",
                        context.Saga.OrderId);
                })
                .Publish(context => (ICompensateOrderCreation) new CompensateOrderCreation {OrderId = context.Saga.OrderId})
                // ошибки в создании заказа не было, но если была ошибка в планировке доставки, можно
                // считать что созданный заказ был создан ошибочно и его нужно удалить - это и есть суть транзакции
                .TransitionTo(OrderCreationFault)
            );
    }
    
}