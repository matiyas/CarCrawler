namespace CarCrawler.Services.Scrapers;

internal class VehicleHistoryReportScraperService
{
    private string _registrationNumber;
    private DateOnly _dateOfFirstRegistration;
    private string _vin;

    public VehicleHistoryReportScraperService(
        string registrationNumber, 
        DateOnly dateOfFirstRegistration,
        string vin)
    {
        _registrationNumber = registrationNumber;
        _dateOfFirstRegistration = dateOfFirstRegistration;
        _vin = vin;
    }

    //public VehicleHistoryReport GetReport()
    //{

    //}
}
