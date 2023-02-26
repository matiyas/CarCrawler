using CarCrawler.Services.Scrapers;
using CsvHelper;
using System.Globalization;

namespace CarCrawler.Services.Generators.CSV;

internal class AdsListReportCsvGeneratorService
{
    private readonly Uri _adsListLink;

    // TODO: Change it to IAdsListProvider or something similar
    public AdsListReportCsvGeneratorService (Uri adsListLink)
    {
        _adsListLink = adsListLink;
    }

    public void Call ()
    {
        using var writer = new StreamWriter("report.csv");
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        foreach (var adDetails in GetAdDetails())
        {
            csv.WriteRecord(adDetails);
            csv.NextRecord();
        }
    }

    private IEnumerable<AdDetails> GetAdDetails ()
    {
        var adListLinksScraperService = new AdListLinksScraperService(_adsListLink);

        foreach (var pageLinks in adListLinksScraperService.GetLinksFromPages())
        {
            foreach (var link in pageLinks)
            {
                var newAdDetails = new AdDetailsScraperService(link).Call();
                if (newAdDetails != null)
                {
                    yield return newAdDetails;
                }
            }
        }
    }
}
