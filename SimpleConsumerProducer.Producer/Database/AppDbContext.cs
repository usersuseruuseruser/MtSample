using MassTransit;
using Microsoft.EntityFrameworkCore;
using SimpleConsumerProducer.Producer.Models;

namespace SimpleConsumerProducer.Producer.Database;

public class AppDbContext: DbContext
{
    // будем считать что продьюсер хранит заказы в базе данных, причем прямо в том виде что и пришли
    public DbSet<OrderDetails> Orders { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        base.OnModelCreating(modelBuilder);
    }
}