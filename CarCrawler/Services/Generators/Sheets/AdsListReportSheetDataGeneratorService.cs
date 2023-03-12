using CarCrawler.Services.Scrapers;

namespace CarCrawler.Services.Generators.Sheets;

internal class AdsListReportSheetDataGeneratorService
{
    private readonly Uri _adsListLink;

    public AdsListReportSheetDataGeneratorService (Uri adsListLink)
    {
        _adsListLink = adsListLink;
    }

    public  IList<IList<object>> Execute ()
    {
        var adDetailsList = GetAdDetails();
        var spreadsheetRows = adDetailsList.Select(adDetails => adDetails.ToGoogleSpreadsheetRow());
        var spreadsheetRowsList = new List<IList<object>>
        {
            AdDetails.SpreadsheetColumns
        };

        spreadsheetRowsList.AddRange(spreadsheetRows);

        return spreadsheetRowsList;
    }

    private IEnumerable<AdDetails> GetAdDetails ()
    {
        var adListLinksScraperService = new AdListLinksScraperService(_adsListLink);

        foreach (var pageLinks in adListLinksScraperService.GetLinksFromPages())
        {
            var pageLinksArray = pageLinks.ToArray();

            for (var i = 0; i < pageLinksArray.Length; i++)
            {
                Console.WriteLine($"---------- Processing link {i + 1}/{pageLinksArray.Length}...");

                var newAdDetails = new AdDetailsScraperService(pageLinksArray[i]).Call();
                if (newAdDetails == null) { continue; }

                yield return newAdDetails;
            }
        }
    }
}
