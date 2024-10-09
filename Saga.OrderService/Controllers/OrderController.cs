using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Contracts;
using Saga.Contracts.OrderRelated;
using Saga.OrderService.Consumers.Models;
using Saga.OrderService.Database;
using Saga.OrderService.Dto;

namespace Saga.OrderService.Controllers;

[ApiController]
public class OrderController: ControllerBase
{
    private readonly IPublishEndpoint _endpoint;
    // used for immediate debugging
    private readonly AppDbContext _dbContext;
    private readonly OrderSagaDbContext _orderSagaDbContext;

    public OrderController(IPublishEndpoint endpoint, AppDbContext dbContext, OrderSagaDbContext orderSagaDbContext)
    {
        _endpoint = endpoint;
        _dbContext = dbContext;
        _orderSagaDbContext = orderSagaDbContext;
    }

    [HttpPost("/order")]
    public async Task<IActionResult> CreateOrder(OrderDto orderDto)
    {
        // создаем новый заказ, добавляем в бд
        // потом добавляем в сервис доставки(считаем количество на выбранном складе, добавляем запись)
        // потом оплачиваем в сервисе оплаты(имитируем оплату, публикуем событие PaymentProcessed)
        // потом отправляем сообещние через сервис уведомлений
        // после этого возвращает результат транзакции
        // -----------------------------------------------------------
        
        //валидация данных, проверка что код оплаты действительно зарезервировал деньги на карте банка клиента
        //так как все тут учебное будет считать что отправили код из дто в апи банка и он вернул что да, у клиента есть
        //деньги и они зарезервированы на данную покупку на наш кошелек
        var orderId = NewId.NextGuid();
        await _endpoint.Publish<IStartOrderSaga>(new StartOrderSaga()
        {
            // надо привязать сагу к id заказа. поэтому создаем заранее. иначе пришлось бы вместе со всей этой инфой
            // идти в консюмера и получать id там(избыток информации для консюмера). можно было решить медиатором,
            // но это уже другая история
            OrderId = orderId,
            Address = orderDto.Address,
            BankPaymentCode = orderDto.BankPaymentCode,
            ClientId = orderDto.ClientId,
            Email = orderDto.Email,
            ItemId = orderDto.ItemId,
            Quantity = orderDto.Quantity,
            WarehouseId = orderDto.WarehouseId
        });
        
        return Ok($"Order created. It's id is {orderId}");
    }

    [HttpGet("/getOrder/{id}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var order = await _dbContext.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        return Ok(order);
    }
    [HttpGet("/checkOrderStatus/{id}")]
    public async Task<IActionResult> CheckOrderStatus(Guid id)
    {
        var data = await _orderSagaDbContext.OrderStates.FindAsync(id);
        if (data == null)
        {
            return NotFound();
        }
        
        return Ok(data);
    }

    [HttpGet("/create-order-manually")]
    public async Task<IActionResult> CreateOrderManually()
    {
        await _endpoint.Publish((ICreateOrder)new CreateOrder()
        {
            OrderId = NewId.NextGuid(),
            ClientId = NewId.NextGuid()
        });
        
        return Ok();
    }
}