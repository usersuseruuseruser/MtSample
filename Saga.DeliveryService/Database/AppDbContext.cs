using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.DeliveryService.Database.Models;

namespace Saga.DeliveryService.Database;

public class AppDbContext: DbContext
{
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Status> Statuses { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // i can assure you dear reader that orderid is unique and is good enough to be a primary key
        modelBuilder.Entity<Delivery>().HasKey(d => d.OrderId);
        
        modelBuilder.AddTransactionalOutboxEntities();
    }
}