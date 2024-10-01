using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Saga.OrderService.Database.Mapping;
using Saga.OrderService.Models;

namespace Saga.OrderService.Database;

public class AppDbContext: DbContext
{
    public DbSet<Order> Orders { get; set; } = null!;

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    
}