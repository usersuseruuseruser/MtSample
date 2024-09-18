using MassTransit;
using SimpleConsumerProducer.Consumer.Consumers;
using SimpleConsumerProducer.Consumer.Definitions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    
    // message redelivery on the whole-project level
    configurator.AddConfigureEndpointsCallback((context,name,cfg) =>
    {
        cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
        cfg.UseMessageRetry(r => r.Immediate(3));
    });
    
    configurator.AddConsumer<OrderConsumer>(typeof(OrderConsumerDefinition));
    
    configurator.UsingRabbitMq((context, factoryConfigurator) =>
    {
        factoryConfigurator.ReceiveEndpoint("orders-queue", e =>
        {
            e.ConfigureConsumer<OrderConsumer>(context);

            // message redelivery on the endpoint level
            e.UseDelayedRedelivery(r => r.Intervals(
                TimeSpan.FromSeconds(5), 
                TimeSpan.FromMinutes(10), 
                TimeSpan.FromMinutes(15)));

            e.UseMessageRetry(r => r.Immediate(3));
        });

        
        factoryConfigurator.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
            
            h.Heartbeat(TimeSpan.FromSeconds(5));
        });
        factoryConfigurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();