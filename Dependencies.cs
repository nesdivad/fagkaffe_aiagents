using Fagkaffe.Services.Gcp;
using Fagkaffe.Tools.Dotnet;
using Fagkaffe.Tools.Files;
using Fagkaffe.Tools.Fruit;
using Fagkaffe.Tools.Geocode;
using Fagkaffe.Tools.Geolocation;
using Fagkaffe.Tools.Transport;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fagkaffe;

public static class ServiceDependencies
{
    public static void RegisterDependencies(this IServiceCollection services)
    {
        services.AddScoped<IGeocodeService, GeocodeService>();
        services.AddScoped<IGeolocationService, GeolocationService>();

        services.AddScoped<IGcpService, GcpService>();
        services.AddHttpClient<IGcpService, GcpService>();

        services.AddScoped<ITransportService, TransportService>();
        services.AddHttpClient<ITransportService, TransportService>(client =>
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("tool-calling github.com/nesdivad");
        });
    }

    public static void RegisterLogging(this IServiceCollection services, LogLevel logLevel = LogLevel.Information)
    {
        services.AddLogging(b => b.AddConsole().SetMinimumLevel(logLevel));
    }

    public static IList<Delegate> GetTools(this IHost app)
    {
        return
        [
            new GeocodeTool(app.Services.GetService<IGeocodeService>()!).GetGeocoordinatesAsync,
            new GeolocationTool(app.Services.GetService<IGeolocationService>()!).GetGeolocationAsync,
            new TransportTool(app.Services.GetService<ITransportService>()!).GetTravelPlanAsync,

            FruitTools.GetFruitColour,
            FileTool.SearchFileAsync,
            FileTool.ReadFileAsync,
            FileTool.WriteFileAsync,
            FileTool.ReplaceTextAsync,
            FileTool.ReverseFileChangesAsync,
            FileTool.SearchTextInFiles,
            DotnetTool.BuildAsync
        ];
    }
}