using MassTransit;
using Microsoft.EntityFrameworkCore;
using SimpleConsumerProducer.Consumer.Models;

namespace SimpleConsumerProducer.Consumer.DbContext;

public class AppDbContext: Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<OrderSentEmail> Emails { get; set; } = null!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.LogTo(Console.WriteLine, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddTransactionalOutboxEntities();
        base.OnModelCreating(modelBuilder);
    }
}