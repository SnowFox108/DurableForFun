using DaprDockerTester.Models;

namespace DaprDockerTester.Services;
public interface IOrderSubmitService
{
    Task SubmitOrder(Basket basket);
}

