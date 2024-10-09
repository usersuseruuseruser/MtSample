namespace Saga.Contracts.DeliveryRelated;

public record DeliveryPlanningCompensated(Guid OrderId, DateTime CompensatedAt);
