using System.Net.Http.Json;
using Fagkaffe.Models;
using Microsoft.Extensions.Configuration;

namespace Fagkaffe.Tools.Transport;

public interface ITransportService
{
    Task<TransportModel?> GetTravelPlanAsync(
        GeoCoordinates fromLocationCoordinates,
        GeoCoordinates toLocationCoordinates,
        string fromLocation,
        string toLocation
    );
}

public class TransportService(HttpClient httpClient, IConfiguration configuration) : ITransportService
{
    private Uri BaseUri { get; } = new(configuration["GIANTLEAP_API_ENDPOINT"]!);

    public async Task<TransportModel?> GetTravelPlanAsync(
        GeoCoordinates fromLocationCoordinates,
        GeoCoordinates toLocationCoordinates,
        string fromLocation,
        string toLocation)
    {
        string utc = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.FFFZ");
        string query = $"FromLocation={fromLocationCoordinates}&ToLocation={toLocationCoordinates}&FromName={fromLocation}&ToName={toLocation}&TimeType=DEPARTURE&TS={utc}&modes=airportbus,bus,carferry,expressbus,others,passengerboat,train,tram&minimumTransferTime=120&walkSpeed=normal";
        Uri uri = new(BaseUri, $"/v5/travelplans?{query}");

        var res = await httpClient.GetAsync(uri);
        var content = await res.Content.ReadFromJsonAsync<TransportModel>();

        return content;
    }
}