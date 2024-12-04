using Fagkaffe.Tools.Geocode;
using Fagkaffe.Tools.Geolocation;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Fagkaffe.Services.Gcp;

public interface IGcpService
{
    Task<GeocodeResponseModel?> GetGeocodeAsync(string location);
    Task<GeolocationModel?> GetGeolocationAsync();
}

public class GcpService(IConfiguration configuration, HttpClient httpClient) : IGcpService
{
    private Uri BaseUri { get; } = new(configuration["GOOGLE_CLOUD_ENDPOINT"]!);
    private Uri BaseMapUri { get; } = new(configuration["GOOGLE_CLOUD_MAP_ENDPOINT"]!);
    private string ApiKey { get; } = configuration["GOOGLE_CLOUD_API_KEY"]!;

    public async Task<GeocodeResponseModel?> GetGeocodeAsync(string location)
    {
        var uri = new Uri(BaseMapUri, $"/maps/api/geocode/json?address={location}&key={ApiKey}");
        return await GetAsync<GeocodeResponseModel>(uri);
    }

    public async Task<GeolocationModel?> GetGeolocationAsync()
    {
        var uri = new Uri(BaseUri, $"/geolocation/v1/geolocate?key={ApiKey}");
        return await PostAsync<GeolocationModel>(uri);
    }

    private async Task<T?> GetAsync<T>(Uri uri)
    {
        return await httpClient.GetFromJsonAsync<T>(uri);
    }

    private async Task<T?> PostAsync<T>(Uri uri, HttpContent? content = null)
    {
        var result = await httpClient.PostAsync(uri, content);
        return await result.Content.ReadFromJsonAsync<T>();
    }
}