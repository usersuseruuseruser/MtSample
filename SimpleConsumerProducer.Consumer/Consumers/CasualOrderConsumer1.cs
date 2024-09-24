using Contracts;
using MassTransit;
using MassTransit.Mediator;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class CasualOrderConsumer1: IConsumer<CasualOrder>
{
    private ILogger<CasualOrderConsumer1> _logger;
    private IMediator _mediator;
    
    public CasualOrderConsumer1(ILogger<CasualOrderConsumer1> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<CasualOrder> context)
    {
        _logger.LogInformation($"Casually consuming order with id {context.Message.Id} and text {context.Message.Order}");
        
        var casualOrderContinuation = new CasualOrderContinuation
        {
            Id = context.Message.Id,
            OrderContinuation = $"Continuation of order {context.Message.Order}"
        };
        // await _mediator.Publish(casualOrderContinuation);

        var client = _mediator.CreateRequestClient<CasualOrderContinuation>();
        var resp = await client.GetResponse<CasualOrderResponce>(casualOrderContinuation);
        
        _logger.LogInformation($"Response from CasualOrderResponce: {resp.Message.OrderResponce}");
    }
}