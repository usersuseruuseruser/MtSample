using MassTransit;
using SimpleConsumerProducer.Consumer.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    
    configurator.AddConsumer<OrderConsumer>(e =>
    {
        e.Options<BatchOptions>(options =>
        {
            options.MessageLimit = 3;
            options.TimeLimit = TimeSpan.FromSeconds(10);
        });
    });
    
    configurator.UsingRabbitMq((context, factoryConfigurator) =>
    {
        factoryConfigurator.Host("rabbitmq://localhost");
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