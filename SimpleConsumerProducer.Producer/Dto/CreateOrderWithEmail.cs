namespace SimpleConsumerProducer.Producer.Dto;

public class CreateOrderWithEmail
{
    public string Email { get; set; } = null!;
    public int Trees { get; set; }
    public string Address { get; set; } = null!;
}