using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Saga.OrderService.Database.Mapping;
using Saga.OrderService.Saga;

namespace Saga.OrderService.Database;

public class OrderSagaDbContext: SagaDbContext
{
    public DbSet<OrderState> OrderStates { get; set; } = null!;


    public OrderSagaDbContext(DbContextOptions<OrderSagaDbContext> options) : base(options)
    {
    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new OrderStateMap();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Sagas");
        modelBuilder.AddTransactionalOutboxEntities();
    }
}