using System.Data;
using System.Reflection;
using MassTransit;
using MassTransit.RabbitMqTransport.Configuration;
using Microsoft.EntityFrameworkCore;
using Saga.DeliveryService.Consumers;
using Saga.DeliveryService.Consumers.Definitions;
using Saga.DeliveryService.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(b =>
{
    b.UseNpgsql("Host=Saga.DeliveryDb;Database=delivery;Username=postgres;Password=postgres",
        o =>
        {
            o.MigrationsHistoryTable("__EFDeliveryMigrationsHistory");
            o.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
        });
});

builder.Services.AddMassTransit(c =>
{
    c.SetKebabCaseEndpointNameFormatter();

    c.AddConsumer<DeliveryPlanningConsumer, DeliveryPlanningConsumerDefinition>();
    c.AddConsumer<DeliveryPlanningCompensationConsumer, DeliveryPlanningCompensationConsumerDefinition>();
    
    c.AddEntityFrameworkOutbox<AppDbContext>(c =>
    {
        c.IsolationLevel = IsolationLevel.RepeatableRead;
        c.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
        c.DisableInboxCleanupService();
        c.UsePostgres();
    });
    
    c.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("admin");
            h.Password("admin");
            
            h.Heartbeat(TimeSpan.FromSeconds(5));
        });
        
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
