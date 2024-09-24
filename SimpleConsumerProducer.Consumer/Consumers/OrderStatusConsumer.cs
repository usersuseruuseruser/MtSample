using Contracts;
using MassTransit;

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
            // fault check
            throw new Exception("Order not found");
        }

        /*if (context.IsResponseAccepted<OrderCreated>())
        {
            // do something
        }*/

        return context.RespondAsync<OrderStatus>(new
        {
            OrderId = context.Message.OrderId,
            Status = "Completed"
        });
    }
}