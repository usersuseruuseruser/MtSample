using MassTransit;

namespace SimpleConsumerProducer.Producer.Models;

public class OrderDetails
{
    public Guid Id { get; set; }
    public string Address { get; set; } = null!;
    public int Trees { get; set; }
};