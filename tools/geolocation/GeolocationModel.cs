using System.ComponentModel;
using System.Text.Json.Serialization;
using Fagkaffe.Models;

namespace Fagkaffe.Tools.Geolocation;

[Description("Data for geocoordinates")]
public class GeolocationModel
{
    [JsonPropertyName("location")]
    [Description("The user's estimated latitude and longitude coordinates, in degrees.")]
    public GeoCoordinates? Location { get; set; }

    [JsonPropertyName("accuracy")]
    [Description("The accuracy of the measured location, in meters. This represents the radius of a circle around the given location.")]
    public float Accuracy { get; set; }
}