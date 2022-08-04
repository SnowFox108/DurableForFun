using System.Threading.Tasks;
using DaprFuncTester.Models;

namespace DaprFuncTester.Services;
public interface IOrderSubmitService
{
    Task SubmitOrder(Basket basket);
}

