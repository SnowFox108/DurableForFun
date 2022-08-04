using System.Threading.Tasks;
using DaprFuncTester.Models;

namespace DaprFuncTester.Services;

public interface IStateBasketService
{
    Task AddToBasket(Fruit fruit);
    Task<Fruit> GetFruit(int id);
    Task RemoveFromBasket(Fruit fruit);
}

