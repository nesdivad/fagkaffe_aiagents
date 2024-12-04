using System.ComponentModel;

namespace Fagkaffe.Tools.Geocode;

[Description("Tool for retrieving geocoordinates, such as latitude or longitude")]
public class GeocodeTool(IGeocodeService geocodeService)
{
    [Description("Get geocoordinates for an address or location")]
    public async Task<GeocodeModel?> GetGeocoordinatesAsync(
        [Description("name of location")] string location)
    {
        return await geocodeService.GetGeocodeAsync(location);
    }
}