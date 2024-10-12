using System.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.PaymentService.Consumers;
using Saga.PaymentService.Consumers.Definitions;
using Saga.PaymentService.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(o =>
{
    o.UseNpgsql("Host=Saga.PaymentDb;Database=payment;Username=postgres;Password=postgres");
});
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddEntityFrameworkOutbox<AppDbContext>(c =>
    {
        c.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
        c.IsolationLevel = IsolationLevel.RepeatableRead;
        c.DisableInboxCleanupService();

        c.UsePostgres();
    });
    x.AddConsumer<OrderPaymentConsumer, OrderPaymentConsumerDefinition>();
    x.AddConsumer<OrderPaymentCompensationConsumer, OrderPaymentCompensationConsumerDefinition>();
    
    x.UsingRabbitMq((ctx,cfr) =>
    {
        cfr.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
        });
        
        cfr.ConfigureEndpoints(ctx);
    });
});
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
