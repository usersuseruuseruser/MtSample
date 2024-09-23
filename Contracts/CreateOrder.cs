namespace Contracts;

// допустим у меня лесоповальная компания и мне приходит заказ на сруб n деревьев от какой-то компании
public record CreateOrder(string Address, int Trees);