using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SimpleConsumerProducer.Producer.Database;
using SimpleConsumerProducer.Producer.Dto;
using SimpleConsumerProducer.Producer.Models;

namespace SimpleConsumerProducer.Producer.Controllers;

[ApiController]
public class OrderController: ControllerBase
{
    private IPublishEndpoint _endpoint;
    private ILogger<OrderController> _logger;
    private ISendEndpointProvider _sendEndpointProvider;
    private AppDbContext _dbContext;

    public OrderController(IPublishEndpoint endpoint, ILogger<OrderController> logger, ISendEndpointProvider sendEndpointProvider, AppDbContext dbContext)
    {
        _endpoint = endpoint;   
        _logger = logger;
        _sendEndpointProvider = sendEndpointProvider;
        _dbContext = dbContext;
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

    [HttpPost("/order-send-with-email")]
    [ProducesResponseType(typeof(OrderCreated), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateOrderWithEmail(CreateOrderWithEmail data)
    {
        var newid = NewId.Next().ToSequentialGuid();
        _dbContext.Orders.Add(new OrderDetails()
        {
            Id = newid,
            Address = data.Address,
            Trees = data.Trees
        });

        await _endpoint.Publish<SendEmailWithOrderDetails>(new
        {
            OrderId = newid,
            Email = data.Email,
            OrderDetails = $"You have ordered {data.Trees} trees to {data.Address}"
        });
        
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Order creation message sent.");
        return Created();
    }
    [HttpGet("/fault-generator")]
    public async Task<IActionResult> GenerateFault()
    {
        _logger.LogInformation("Generating a fault");
        await _endpoint.Publish(new GenerateFaultOrder(){Trees = 1, Address = "test"});
        _logger.LogInformation("Fault generated");
        return Ok();
    }
}