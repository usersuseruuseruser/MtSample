using Contracts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SimpleConsumerProducer.Consumer.Consumers;
using SimpleConsumerProducer.Consumer.DbContext;
using SimpleConsumerProducer.Consumer.Definitions;
using MassTransit.EntityFrameworkCoreIntegration;
using SimpleConsumerProducer.Consumer.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseNpgsql("Host=PgConsumer;Database=emails;Username=postgres;Password=postgres");
});

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    
    // message redelivery on the whole-project level
    // configurator.AddConfigureEndpointsCallback((context,name,cfg) =>
    // {
    //     cfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(15), TimeSpan.FromMinutes(30)));
    //     cfg.UseMessageRetry(r => r.Immediate(3));
    // });
    
    configurator.AddEntityFrameworkOutbox<AppDbContext>(c =>
    {
        c.QueryTimeout = TimeSpan.FromSeconds(10);
        c.QueryDelay = TimeSpan.FromSeconds(5);
        c.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
        c.DisableInboxCleanupService();
        c.UsePostgres();
    });

    configurator.AddConsumer<OrderWithEmailConsumer>(typeof(OrderWithEmailDefinition));
    configurator.AddConsumer<OrderConsumer>(typeof(OrderConsumerDefinition));
    configurator.AddConsumer<FaultOrderConsumer>();
    configurator.AddConsumer<OrderStatusConsumer>();
    configurator.AddConsumer<CasualOrderConsumer1>();
    
    configurator.AddMediator(c =>
    {
        c.AddConsumer<CasualOrderConsumer1>();
        c.AddConsumer<CasualOrderHelper1>();
        c.AddConsumer<CasualOrderHelper2>();
        c.AddConsumer<CasualOrderResponcer>();
    });
    
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
            
            h.Heartbeat(TimeSpan.FromSeconds(500));
        });
        factoryConfigurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// create and apply migrations
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

// Configure the HTTP request pipeline.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();