using CarCrawler.Services.Calculators;
using CarCrawler.Services.Scrapers;
using System.Numerics;

namespace CarCrawler.Services.Generators.Sheets;

internal class AdsListReportSheetDataGeneratorService
{
    private readonly Uri _adsListLink;
    private readonly DistanceMatrixCalculator _distanceMatrixCalculator;
    private readonly Vector2 _originCoordinates;

    public AdsListReportSheetDataGeneratorService (
        Uri adsListLink, 
        DistanceMatrixCalculator distanceMatrixCalculator, 
        Vector2 originCoords
    )
    {
        _adsListLink = adsListLink;
        _distanceMatrixCalculator = distanceMatrixCalculator;
        _originCoordinates = originCoords;
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

                // TODO: Move it to distinct class
                var sellerCoordinates = newAdDetails?.SellerCoordinates;
                if (sellerCoordinates != null)
                {
                    var distanceMatrix = _distanceMatrixCalculator.Calculate(_originCoordinates, sellerCoordinates.Value);

                    if (distanceMatrix != null)
                    {
                        newAdDetails!.TravelDuration = distanceMatrix.Duration;
                        newAdDetails!.TravelDistance = distanceMatrix.DistanceMeters;
                    }
                }


                yield return newAdDetails!;
            }
        }
    }
}
