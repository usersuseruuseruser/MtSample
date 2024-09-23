namespace Contracts;

public class SendEmailWithOrderDetails
{
    public Guid OrderId { get; set; }
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = "Order details";
    public string OrderDetails { get; set; } = null!;
}