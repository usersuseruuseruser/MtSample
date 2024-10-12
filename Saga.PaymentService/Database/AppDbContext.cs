using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.PaymentService.Database.Models;

namespace Saga.PaymentService.Database;

public class AppDbContext: DbContext
{
    public DbSet<PaidOrder> PaidOrders { get; set; } = null!;
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddTransactionalOutboxEntities();
    }
}