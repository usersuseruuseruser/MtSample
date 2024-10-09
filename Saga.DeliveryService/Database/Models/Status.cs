using Saga.DeliveryService.Database.Models.Enums;

namespace Saga.DeliveryService.Database.Models;

public class Status
{
    public Guid Id { get; set; }
    public DeliveryStatus Current { get; set; }
}