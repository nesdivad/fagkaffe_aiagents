using Fagkaffe.Services.Gcp;

namespace Fagkaffe.Tools.Geolocation;

public interface IGeolocationService
{
    Task<GeolocationModel?> GetGeolocationAsync();
}

public class GeolocationService(IGcpService gcpService) : IGeolocationService
{
    public async Task<GeolocationModel?> GetGeolocationAsync()
    {
        var result = await gcpService.GetGeolocationAsync();
        return result;
    }
}