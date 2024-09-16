namespace Contracts;

// после того как заказ был создан, помимо изначальной информации к нему добавляется дата, когда он будет сделан
public record OrderCreated(string Company, int Trees, DateTime FinishedAt);