using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saga.OrderService.Saga;

namespace Saga.OrderService.Database.Mapping;

public class OrderStateMap: SagaClassMap<OrderState>
{
    protected override void Configure(EntityTypeBuilder<OrderState> entity, ModelBuilder model)
    {
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.ItemId).IsRequired();
        entity.Property(x => x.ClientId).IsRequired();
        entity.Property(x => x.Quantity).IsRequired();
        entity.Property(x => x.Address).HasMaxLength(256);
        entity.Property(x => x.Email).HasMaxLength(256);
        entity.Property(x => x.WarehouseId).HasMaxLength(64);
        entity.Property(x => x.BankPaymentCode).HasMaxLength(64);
        
        // it will make CorrelationId primary key by convention
        // usually it's like this entity.Property(x => x.RowVersion).IsRowVersion();
        // but postgres has hidden row version column. we'll map our RowVersion to it
        entity.Property(x => x.RowVersion)
            .HasColumnName("xmin")
            .HasColumnType("xid")
            .IsRowVersion();
    }
}