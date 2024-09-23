namespace SimpleConsumerProducer.Consumer.Models;

public class OrderSentEmail
{
    public Guid Id { get; set; }
    public string EmailAdress { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Body { get; set; } = null!;
}