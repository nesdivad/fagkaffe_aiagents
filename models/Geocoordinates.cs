using System.ComponentModel;
using System.Text.Json.Serialization;
using Fagkaffe.Helpers;

namespace Fagkaffe.Models;

public class GeoCoordinates
{
    [JsonPropertyName("lat")]
    [Description("Latitude of location, in degrees.")]
    public double Latitude { get; set; }

    [JsonPropertyName("lng")]
    [Description("Longitude of location, in degrees.")]
    public double Longitude { get; set; }

    public override string ToString()
    {
        var latStr = EverythingHelper.DoubleToString(Latitude);
        var lonStr = EverythingHelper.DoubleToString(Longitude);

        return $"{latStr},{lonStr}";
    }
}