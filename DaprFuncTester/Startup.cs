using DaprFuncTester.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(FunctionDurable.Startup))]
namespace FunctionDurable
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDaprClient();
            builder.Services.AddScoped<IStateBasketService, DaprClientStateBasketService>();
            builder.Services.AddScoped<IOrderSubmitService, DaprOrderSubmitService>();


        }
    }
}
