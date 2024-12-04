using System.Text.Json.Serialization;
using Fagkaffe.Models;

namespace Fagkaffe.Tools.Geocode;

public class GeocodeResponseModel
{
    [JsonPropertyName("results")]
    public IList<GeocodeModel> Results { get; set; } = [];

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class GeocodeModel
{
    [JsonPropertyName("address_components")]
    public IList<Address> Addresses { get; set; } = [];

    [JsonPropertyName("geometry")]
    public Geometry? Geometry { get; set; }

    [JsonPropertyName("place_id")]
    public string? PlaceId { get; set; }
}

public class Address
{
    [JsonPropertyName("long_name")]
    public string? LongName { get; set; }

    [JsonPropertyName("short_name")]
    public string? ShortName { get; set; }

    [JsonPropertyName("types")]
    public IList<string> Types { get; set; } = [];
}

public class Geometry
{
    [JsonPropertyName("location")]
    public GeoCoordinates? Location { get; set; }

    [JsonPropertyName("location_type")]
    public string? LocationType { get; set; }
}