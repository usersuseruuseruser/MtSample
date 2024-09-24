using Contracts;
using MassTransit;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class OrderStatusConsumer: IConsumer<CheckOrderStatus>
{
    private ILogger<OrderStatusConsumer> _logger;

    public OrderStatusConsumer(ILogger<OrderStatusConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<CheckOrderStatus> context)
    {
        _logger.LogInformation("Checking order status for order {OrderId}", context.Message.OrderId);

        if (context.Message.OrderId > 100)
        {
            // будем считать что это типо эмуляция падения бд, то есть совершенно непредвиденная ситуация с нашей стороны
            // такое мы выкидываем в виде исключений. 
            throw new ArgumentException("бд умерла", nameof(context.Message.OrderId));
        }

        /*if (context.IsResponseAccepted<OrderCreated>())
        {
            // do something
        }*/
        
        // эмуляция того что заказ не найден
        if (Random.Shared.Next(0,2) != 0)
        {
          return context.RespondAsync(new OrderNotFound(){OrderId = context.Message.OrderId});   
        }
        
        return context.RespondAsync<OrderStatus>(new
        {
            OrderId = context.Message.OrderId,
            Status = "Completed"
        });
    }
}