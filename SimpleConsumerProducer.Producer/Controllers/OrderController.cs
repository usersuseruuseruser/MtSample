using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SimpleConsumerProducer.Producer.Dto;

namespace SimpleConsumerProducer.Producer.Controllers;

[ApiController]
public class OrderController: ControllerBase
{
    private IPublishEndpoint _endpoint;
    private ILogger<OrderController> _logger;

    public OrderController(IPublishEndpoint endpoint, ILogger<OrderController> logger)
    {
        _endpoint = endpoint;
        _logger = logger;
    }

    [HttpPost("/order")]
    [ProducesResponseType(typeof(OrderCreated),StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrder(CreateOrder order)
    {
        // validate order somehow, simple one:
        if (order.Trees <= 0 || order.Trees >= 100)
        {
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 403,
                Message = "Введено некорректное число деревьев"
            });
        }
        _logger.LogInformation("Sending an order creation message for {Trees} trees.", order.Trees);
        await _endpoint.Publish(order);
        _logger.LogInformation("Order creation message sent.");
        return Created();
    }
}