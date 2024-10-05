using System.Data;
using System.Reflection;
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
    optionsAction.UseNpgsql("Host=Saga.OrderSaga;Database=orders;Username=postgres;Password=postgres",
        o =>
        {
            o.MigrationsHistoryTable("__EFOrdersMigrationsHistory", "Orders");
            o.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
});
builder.Services.AddDbContext<OrderSagaDbContext>(builder =>
{
    builder.UseNpgsql(
        "Host=Saga.OrderSaga;Database=orders;Username=postgres;Password=postgres",
        o =>
        {
            o.MigrationsHistoryTable("__EFSagasMigrationsHistory", "Sagas");
            o.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
});
builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    configurator.AddConsumers(Assembly.GetExecutingAssembly());
    configurator.AddEntityFrameworkOutbox<OrderSagaDbContext>(c =>
    {
        c.IsolationLevel = IsolationLevel.RepeatableRead;
        c.QueryTimeout = TimeSpan.FromSeconds(5);
        c.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
        c.DisableInboxCleanupService();
        c.UsePostgres();
    });
    
    configurator.AddSagaStateMachine<OrderStateMachine, OrderState>(typeof(OrderSagaDefinition))
        .EntityFrameworkRepository(r =>
        {
            r.ConcurrencyMode = ConcurrencyMode.Optimistic;
            r.IsolationLevel = IsolationLevel.RepeatableRead;
            r.ExistingDbContext<OrderSagaDbContext>();
            r.UsePostgres();
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

await using (var scope = app.Services.CreateAsyncScope())
{
    // вынести в hosted service
    var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var sagaDbContext = scope.ServiceProvider.GetRequiredService<OrderSagaDbContext>();
    await appDbContext.Database.MigrateAsync();
    await sagaDbContext.Database.MigrateAsync();
}
app.MapControllers();
app.Run();
