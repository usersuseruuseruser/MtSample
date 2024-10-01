namespace Saga.Contracts;

public interface IOrderCreationCompensated
{
    public Guid OrderId { get; }
    public DateTime CompensatedAt { get; }
}