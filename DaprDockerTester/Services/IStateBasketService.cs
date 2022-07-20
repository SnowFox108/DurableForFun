using DaprDockerTester.Models;

namespace DaprDockerTester.Services;

public interface IStateBasketService
{
    Task AddToBasket(Fruit fruit);
    Task<Fruit> GetFruit(int id);
    Task RemoveFromBasket(Fruit fruit);
}

