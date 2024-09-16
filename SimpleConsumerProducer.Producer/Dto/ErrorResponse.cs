namespace SimpleConsumerProducer.Producer.Dto;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string? Message { get; set; } 
    public List<string>? Details { get; set; }
}