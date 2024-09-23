using MassTransit;
using Microsoft.EntityFrameworkCore;
using SimpleConsumerProducer.Producer.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql("Host=PgProducer;Database=orders;Username=postgres;Password=postgres");
});
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();
    
    configurator.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.QueryTimeout = TimeSpan.FromSeconds(30);
        o.QueryDelay = TimeSpan.FromSeconds(3);
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(5);
        o.DisableInboxCleanupService();
        o.UsePostgres().UseBusOutbox();
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

// create and apply migrations
using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
dbContext.Database.Migrate();

// Configure the HTTP request pipeline.
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
