using Fagkaffe.Services.Gcp;
using Microsoft.Extensions.Logging;

namespace Fagkaffe.Tools.Geocode;

public interface IGeocodeService
{
    Task<GeocodeModel?> GetGeocodeAsync(string address);
}

public class GeocodeService(IGcpService gcpService, ILogger<IGeocodeService> logger) : IGeocodeService
{
    public async Task<GeocodeModel?> GetGeocodeAsync(string address)
    {
        logger.LogInformation($"Sending request for address: {address}");

        var result = await gcpService.GetGeocodeAsync(address);
        if (result is null || result.Status != "OK" || result.Results.Count is 0)
        {
            return null;
        }

        return result.Results[0];
    }
}