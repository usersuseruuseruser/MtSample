using MassTransit;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class CasualOrderHelper1: IConsumer<CasualOrderContinuation>
{
    private ILogger<CasualOrderHelper1> _logger;

    public CasualOrderHelper1(ILogger<CasualOrderHelper1> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CasualOrderContinuation> context)
    {
        await context.RespondAsync(new CasualOrderResponce() { OrderResponce = "test" });
        _logger.LogInformation($"Casually consuming order with id {context.Message.Id} and text {context.Message.OrderContinuation}");
    }
}