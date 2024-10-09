namespace Saga.Contracts.DeliveryRelated;

public record DeliveryPlanned(Guid OrderId, DateTime PlannedAt);