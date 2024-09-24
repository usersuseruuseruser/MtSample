using Contracts;
using MassTransit;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class FaultOrderConsumer: IConsumer<GenerateFaultOrder>
{
    public Task Consume(ConsumeContext<GenerateFaultOrder> context)
    {
        throw new Exception("Random exception");
    }
}