using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Fagkaffe.Tools.Transport;

[Description("Object containing result code and list of travel plans")]
public class TransportModel
{
    [JsonPropertyName("resultCode")]
    [Description("Whether response was a success or not")]
    public string? ResultCode { get; set; }

    [JsonPropertyName("TravelPlans")]
    [Description("List of travel plans")]
    public IList<TravelPlan>? TravelPlans { get; set; }
}

public class TravelPlan
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("StartTime")]
    [Description("Start time of travel plan, denoted in UTC")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("EndTime")]
    [Description("End time of travel plan, denoted in UTC")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("TravelSteps")]
    [Description("List of travel steps in plan")]
    public IList<TravelStep>? TravelSteps { get; set; }

    [JsonPropertyName("End")]
    [Description("Description of end stop")]
    public EndStop? EndStop { get; set; }

    [JsonPropertyName("shortTransferTime")]
    [Description("Whether the travel plan has a short transfer time between steps")]
    public bool ShortTransferTime { get; set; }
}

public class EndStop : Stop
{
    [JsonPropertyName("Location")]
    [Description("Location in latitude and longitude")]
    public string? Location { get; set; }
}

public class TravelStep
{
    [JsonPropertyName("Type")]
    [Description("Type of travel step")]
    public string? Type { get; set; }

    [JsonPropertyName("StartTime")]
    [Description("Start time of travel plan, denoted in UTC")]
    public DateTime StartTime { get; set; }

    [JsonPropertyName("EndTime")]
    [Description("End time of travel plan, denoted in UTC")]
    public DateTime EndTime { get; set; }

    [JsonPropertyName("Distance")]
    [Description("Distance of travel step, in meters")]
    public int Distance { get; set; }

    [JsonPropertyName("StopIdentifier")]
    [Description("Id for stop")]
    public string? StopIdentifier { get; set; }

    [JsonPropertyName("Status")]
    [Description("Status of current travel step")]
    public string? Status { get; set; }

    [JsonPropertyName("RouteDirection")]
    [Description("Direction of route")]
    public RouteDirection? RouteDirection { get; set; }

    [JsonPropertyName("Stop")]
    [Description("Information about the stop")]
    public Stop? Stop { get; set; }

    [JsonPropertyName("ExpectedEndTime")]
    [Description("Datetime for expected end of travel step")]
    public DateTime ExpectedEndTime { get; set; }

    [JsonPropertyName("Passed")]
    [Description("The transport has passed the starting stop")]
    public bool Passed { get; set; }
}

public class RouteDirection
{
    [JsonPropertyName("PublicIdentifier")]
    [Description("Public identifier of transport")]
    public string? PublicIdentifier { get; set; }

    [JsonPropertyName("DirectionName")]
    [Description("Information about what direction the transport is headed")]
    public string? DirectionName { get; set; }

    [JsonPropertyName("ServiceMode")]
    [Description("Kind of transport")]
    public string? ServiceMode { get; set; }
}

public class Stop
{
    [JsonPropertyName("Identifier")]
    [Description("Identifier of stop")]
    public string? Identifier { get; set; }

    [JsonPropertyName("Description")]
    [Description("Description of stop")]
    public string? Description { get; set; }

    [JsonPropertyName("ExtraText")]
    [Description("Additional info about the stop")]
    public string? ExtraText { get; set; }

    [JsonPropertyName("Platform")]
    [Description("Information about the platform")]
    public string? Platform { get; set; }
}