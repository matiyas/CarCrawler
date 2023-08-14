namespace CarCrawler.Converters;

internal static class VehicleHistoryReportConverter
{
    public static VehicleHistoryReport Convert(
        VehicleHistoryReportFetcher.Models.VehicleHistoryReport vehicleHistoryReportSrc)
    {
        return new VehicleHistoryReport
        {
            NumberOfOwnersInTheCountry = vehicleHistoryReportSrc.NumberOfOwnersInTheCountry,
            FirstRegistrationAbroad = vehicleHistoryReportSrc.FirstRegistrationAbroad,
            FirstRegistrationInTheCountry = vehicleHistoryReportSrc.FirstRegistrationInTheCountry
        };
    }
}