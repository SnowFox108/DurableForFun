using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using PdfGenerator;
using PdfSharpCore.Fonts;

[assembly: FunctionsStartup(typeof(Startup))]
namespace PdfGenerator
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            GlobalFontSettings.FontResolver = new AzureFontResolver();
            //builder.Services.AddHttpClient();

        }
    }
}
