using MassTransit;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class CasualOrderHelper2: IConsumer<CasualOrderContinuation>
{
    private ILogger<CasualOrderHelper2> _logger;

    public CasualOrderHelper2(ILogger<CasualOrderHelper2> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CasualOrderContinuation> context)
    {
        _logger.LogInformation($"Casually consuming order with id {context.Message.Id} and text {context.Message.OrderContinuation}");
    }
}