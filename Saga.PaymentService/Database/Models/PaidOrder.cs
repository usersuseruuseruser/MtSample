using System.ComponentModel.DataAnnotations;

namespace Saga.PaymentService.Database.Models;

public class PaidOrder
{
    [Key]
    public Guid OrderId { get; set; }

    public string BankPaymentCode { get; set; } = null!;
    
    public DateTime PaymentDate { get; set; }
}