using System.ComponentModel;

namespace Fagkaffe.Tools.Geolocation;

[Description("Tool for retrieving the user's current location")]
public class GeolocationTool(IGeolocationService geolocationService)
{
    [Description("Get user's location in geocoordinates")]
    public async Task<GeolocationModel?> GetGeolocationAsync()
    {
        var result = await geolocationService.GetGeolocationAsync();
        return result;
    }
}