using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Contracts;
using Saga.OrderService.Consumers.Models;
using Saga.OrderService.Database;
using Saga.OrderService.Dto;
using Saga.OrderService.Enums;
using Saga.OrderService.Models;

namespace Saga.OrderService.Controllers;

[ApiController]
public class OrderController: ControllerBase
{
    private readonly IPublishEndpoint _endpoint;

    public OrderController(IPublishEndpoint endpoint)
    {
        _endpoint = endpoint;
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
        await _endpoint.Publish<IStartOrderSaga>(new StartOrderSaga()
        {
            // надо привязать сагу к id заказа. поэтому создаем заранее. иначе пришлось бы вместе со всей этой инфой
            // идти в консюмера и получать id там(избыток информации для консюмера). можно было решить медиатором,
            // но это уже другая история
            OrderId = new Guid(),
            Address = orderDto.Address,
            BankPaymentCode = orderDto.BankPaymentCode,
            ClientId = orderDto.ClientId,
            Email = orderDto.Email,
            ItemId = orderDto.ItemId,
            Quantity = orderDto.Quantity,
            WarehouseId = orderDto.WarehouseId
        });
        
        return Created();
    }
}