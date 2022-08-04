using System.Threading.Tasks;
using Dapr.Client;
using DaprFuncTester.Models;
using Microsoft.Extensions.Logging;

namespace DaprFuncTester.Services;

public class DaprOrderSubmitService : IOrderSubmitService
{
    private readonly DaprClient _client;
    private readonly ILogger<DaprOrderSubmitService> _logger;

    public DaprOrderSubmitService(
        DaprClient client, 
        ILogger<DaprOrderSubmitService> logger)
    {
        _client = client;
        _logger = logger;
    }


    public async Task SubmitOrder(Basket basket)
    {
        _logger.LogInformation("Posting order event to Dapr pubsub");
        await _client.PublishEventAsync("redis-pubsub", "orders", basket);
    }
}

