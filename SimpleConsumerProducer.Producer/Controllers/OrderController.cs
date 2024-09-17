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
    private ISendEndpointProvider _sendEndpointProvider;

    public OrderController(IPublishEndpoint endpoint, ILogger<OrderController> logger, ISendEndpointProvider sendEndpointProvider)
    {
        _endpoint = endpoint;   
        _logger = logger;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpPost("/order-publish")]
    [ProducesResponseType(typeof(OrderCreated),StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrderPublish(CreateOrder order)
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
        _logger.LogInformation("Publishing an order creation message for {Trees} trees.", order.Trees);
        await _endpoint.Publish(order);
        _logger.LogInformation("Order creation message published.");
        return Created();
    }
    
    [HttpPost("/order-send")]
    [ProducesResponseType(typeof(OrderCreated),StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrderSend(CreateOrder order)
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
        // actually the same as publish, but we could send it to the queue directly
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:Contracts:CreateOrder"));
        await endpoint.Send(order, context => context.Headers.Set("x-custom-header-test","some-value"));
        _logger.LogInformation("Order creation message sent.");
        return Created();
    }
}