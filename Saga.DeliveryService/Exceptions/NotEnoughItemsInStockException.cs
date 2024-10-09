namespace Saga.DeliveryService.Exceptions;

public class NotEnoughItemsInStockException: Exception
{
    public NotEnoughItemsInStockException(Guid orderId, Guid itemId, int quantity):
        base($"Not enough items in stock for order {orderId}, item {itemId}, quantity {quantity}")
    {
    }
}