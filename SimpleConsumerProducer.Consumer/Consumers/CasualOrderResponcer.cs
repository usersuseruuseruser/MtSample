using MassTransit;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class CasualOrderResponcer: IConsumer<CasualOrderContinuation>
{
    private ILogger<CasualOrderResponcer> _logger;

    public CasualOrderResponcer(ILogger<CasualOrderResponcer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CasualOrderContinuation> context)
    {
        await context.RespondAsync(new CasualOrderResponce() { OrderResponce = "test12312" });
    }
}