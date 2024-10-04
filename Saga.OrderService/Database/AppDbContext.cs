using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Saga.OrderService.Database.Mapping;
using Saga.OrderService.Models;

namespace Saga.OrderService.Database;

public class AppDbContext: DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Orders");
    }
}