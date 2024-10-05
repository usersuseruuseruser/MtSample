using Contracts;
using MassTransit;
using SimpleConsumerProducer.Consumer.DbContext;
using SimpleConsumerProducer.Consumer.Models;
using CasualOrder = Contracts.CasualOrder;

namespace SimpleConsumerProducer.Consumer.Consumers;

public class OrderWithEmailConsumer
    : IConsumer<SendEmailWithOrderDetails>
{
    private AppDbContext _dbContext;
    private ILogger<OrderWithEmailConsumer> _logger;

    public OrderWithEmailConsumer(AppDbContext dbContext, ILogger<OrderWithEmailConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SendEmailWithOrderDetails> context)
    {
        _logger.LogInformation("Sending email to {Email} with order details", context.Message.Email);
        
        _dbContext.Emails.Add(new OrderSentEmail()
        {
            EmailAdress = context.Message.Email,
            Subject = "Order details",
            Body = context.Message.OrderDetails
        });
        await context.Publish(new CasualOrder(){Order = "test"});
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Email sent to {Email}", context.Message.Email);
    }
}