namespace VehicleHistoryReportFetcher.Models;

public class VehicleHistoryReport
{
    public int? NumberOfOwnersInTheCountry { get; set; }
    public DateOnly? FirstRegistrationAbroad { get; set; }
    public DateOnly? FirstRegistrationInTheCountry { get; set; }
}