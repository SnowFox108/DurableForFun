using System.Text;
using System.Text.Json;
using Dapr.Client;
using DaprWebTester.Models;

namespace DaprWebTester.Services;

public class DaprClientStateBasketService : IStateBasketService
{
    private readonly DaprClient _client;
    private readonly ILogger<DaprClientStateBasketService> _logger;
    private const string baseUrl = "http://localhost:";
    private readonly string? _dapr_port;
    private const string stateStoreName = "statestore";

    public DaprClientStateBasketService(
        DaprClient client,
        ILogger<DaprClientStateBasketService> logger)
    {
        _client = client;
        _dapr_port = "3500"; // Environment.GetEnvironmentVariable("DAPR_HTTP_PORT");
        //_client = new DaprClientBuilder().Build();
        _logger = logger;
    }
    public async Task AddToBasket(Fruit fruit)
    {
        _logger.LogInformation($"Add fruit id:{fruit.Id} name:{fruit.Name} to basket");
        //await SaveStateSdk(fruit);
        await SaveStateHttp(fruit);
    }

    public async Task<Fruit> GetFruit(int id)
    {
        _logger.LogInformation($"Get fruit id:{id} from basket");
        //return await GetStateSdk(id);
        return await GetStateHttp(id);
    }

    public Task RemoveFromBasket(Fruit fruit)
    {
        throw new NotImplementedException();
    }

    private async Task<Fruit> GetStateSdk(int id)
    {
        var key = $"fruit-{id}";
        var fruit = await _client.GetStateAsync<Fruit>(stateStoreName, key);

        return fruit;
    }

    private async Task<Fruit> GetStateHttp(int id)
    {
        var httpClient = new HttpClient();

        var url = $"{baseUrl}{_dapr_port}/v1.0/state/{stateStoreName}/{id.ToString()}";

        var response = await httpClient.GetStringAsync(url);
        var fruit = JsonSerializer.Deserialize<Fruit>(response);

        return fruit;
    }
    private async Task SaveStateSdk(Fruit fruit)
    {
        var key = $"fruit-{fruit.Id}";
        await _client.SaveStateAsync(stateStoreName, key, fruit);
        _logger.LogInformation($"Created new basket in state store {key}");
    }

    private async Task SaveStateHttp(Fruit fruit)
    {
        var httpClient = new HttpClient();
        
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var fruitJson = JsonSerializer.Serialize(new[] {
            new {
                key = fruit.Id,
                value = fruit.Name
            }
        });
        var state = new StringContent(fruitJson, Encoding.UTF8, "application/json");
        var url = $"{baseUrl}{_dapr_port}/v1.0/state/{stateStoreName}";
        await httpClient.PostAsync(url, state);
    }
}

