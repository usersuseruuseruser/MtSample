using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SimpleConsumerProducer.Consumer.Models;
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
    private IRequestClient<CheckOrderStatus> _requestClient;
    private AppDbContext _dbContext;


    public OrderController(IPublishEndpoint endpoint, ILogger<OrderController> logger,
        ISendEndpointProvider sendEndpointProvider, AppDbContext dbContext,
        IRequestClient<CheckOrderStatus> requestClient)
    {
        _endpoint = endpoint;
        _logger = logger;
        _sendEndpointProvider = sendEndpointProvider;
        _dbContext = dbContext;
        _requestClient = requestClient;
    }

    [HttpPost("/order-publish")]
    [ProducesResponseType(typeof(OrderCreated), StatusCodes.Status201Created)]
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
    [ProducesResponseType(typeof(OrderCreated), StatusCodes.Status201Created)]
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
        await endpoint.Send(order, context => context.Headers.Set("x-custom-header-test", "some-value"));
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
        await _endpoint.Publish(new GenerateFaultOrder() { Trees = 1, Address = "test" });
        _logger.LogInformation("Fault generated");
        return Ok();
    }

    [HttpGet("/check-order-status")]
    public async Task<IActionResult> CheckOrderStatus(int id)
    {
        _logger.LogInformation("Checking order status");
        // Apparently, if exception is thrown from the consumer, it cant be used in a Fault<> response
        // We should use a try catch block in order to handle it. like this
        try
        {
            var resp = await _requestClient.GetResponse<OrderStatus, OrderNotFound>(
                new CheckOrderStatus() { OrderId = id },
                x => x.UseExecute(c =>
                    c.Headers.Set("x-custom-header-test", "some-value")));
            if (resp.Is<OrderNotFound>(out Response<OrderNotFound>? notFoundResponce))
            {
                return NotFound(new ErrorResponse()
                {
                    StatusCode = 404,
                    Message = $"Order with id {notFoundResponce.Message.OrderId} not found"
                });
            }
            
            if (resp.Is(out Response<OrderStatus>? properResponse))
            {
                return Ok(properResponse.Message.Status);
            }
        
            _logger.LogError("Unknown response type:" + resp.GetType().Name);
            return StatusCode(500, "Something went wrong, try again later");
        }
        catch (RequestFaultException ex)
        {
            _logger.LogError("Error checking order status: {Error}", ex.Message);
            return BadRequest(new ErrorResponse()
            {
                StatusCode = 500,
                Message = "Error checking order status"
            });
        }
    }
    [HttpPost("/order-casual")]
    public async Task<IActionResult> CreateCasualOrder(CasualOrder order)
    {
        _logger.LogInformation("Publishing a casual order creation message for {Order}", order.Order);
        await _endpoint.Publish(order);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Casual order creation message published.");
        return Created();
    }
}