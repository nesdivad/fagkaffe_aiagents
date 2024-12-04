using System.ComponentModel;
using Fagkaffe.Helpers;
using Fagkaffe.Models;

namespace Fagkaffe.Tools.Transport;

[Description("Tool for fetching information about travel plans using public transport.")]
public class TransportTool(ITransportService transportService)
{
    [Description("Returns travel plans for public transport")]
    public async Task<TransportModel?> GetTravelPlanAsync(
        [Description("Latitude for start location")] double fromLocationLatitude,
        [Description("Longitude for start location")] double fromLocationLongitude,
        [Description("Latitude for end location")] double toLocationLatitude,
        [Description("Longitude for end location")] double toLocationLongitude,
        [Description("Name of start location")] string fromLocationName,
        [Description("Name of end location")] string toLocationName
    )
    {
        GeoCoordinates fromCoordinates = new() { Latitude = fromLocationLatitude, Longitude = fromLocationLongitude };
        GeoCoordinates toCoordinates = new() { Latitude = toLocationLatitude, Longitude = toLocationLongitude };

        var res = await transportService.GetTravelPlanAsync(
            fromCoordinates,
            toCoordinates,
            fromLocationName,
            toLocationName
        );

        if (res is null)
            return null;

        foreach (var plan in res.TravelPlans ?? [])
        {
            plan.StartTime = EverythingHelper.ConvertToCEST(plan.StartTime);
            plan.EndTime = EverythingHelper.ConvertToCEST(plan.EndTime);

            foreach (var step in plan.TravelSteps ?? [])
            {
                step.StartTime = EverythingHelper.ConvertToCEST(step.StartTime);
                step.EndTime = EverythingHelper.ConvertToCEST(step.EndTime);
            }
        }

        return res;
    }
}