using System.Data;
using System.Security.Cryptography.Xml;
using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Saga.OrderService.Database;
using Saga.OrderService.Models;
using Saga.OrderService.Saga;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(optionsAction =>
{
    optionsAction.UseNpgsql("Host=Saga.OrderService;Database=orders;Username=postgres;Password=postgres;Port=5434");
});
builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddSagaStateMachine<OrderStateMachine, OrderState>()
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            r.IsolationLevel = IsolationLevel.RepeatableRead;
            r.AddDbContext<DbContext, SagaDbContext>((provider, builder) =>
            {
                builder.UseNpgsql(
                    "Host=Saga.OrderService;Database=orders;Username=postgres;Password=postgres;Port=5434");
            });
        });
    
    configurator.UsingRabbitMq((context, factoryConfigurator) =>
    {
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using var scope = app.Services.CreateAsyncScope();

// вынести в hosted service
var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
var sagaDbContext = scope.ServiceProvider.GetRequiredService<SagaDbContext>();
await appDbContext.Database.MigrateAsync();
await sagaDbContext.Database.MigrateAsync();

app.UseHttpsRedirection();

app.Run();
