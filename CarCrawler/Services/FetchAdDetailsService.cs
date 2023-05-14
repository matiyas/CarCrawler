using CarCrawler.Database;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Scrapers;
using NetTopologySuite.Geometries;

namespace CarCrawler.Services;

internal class FetchAdDetailsService
{
    private readonly Uri _adsListLink;
    private readonly IDistanceMatrixCalculator _distanceMatrixCalculator;
    private readonly Point _originCoordinates;

    public FetchAdDetailsService(
        Uri adsListLink,
        IDistanceMatrixCalculator distanceMatrixCalculator,
        Point originCoords
    )
    {
        _adsListLink = adsListLink;
        _distanceMatrixCalculator = distanceMatrixCalculator;
        _originCoordinates = originCoords;
    }

    public IEnumerable<AdDetails> Execute()
    {
        var adListLinksScraperService = new AdListLinksScraperService(_adsListLink);

        foreach (var pageLinks in adListLinksScraperService.GetLinksFromPages())
        {
            var pageLinksArray = pageLinks.ToArray();

            for (var i = 0; i < pageLinksArray.Length; i++)
            {
                Logger.Log($"Processing link {i + 1}/{pageLinksArray.Length}...");

                var newAdDetails = new AdDetailsScraperService(pageLinksArray[i]).Call();

                if (newAdDetails == null) { continue; }

                // TODO: Move it to distinct class
                var sellerCoordinates = newAdDetails?.SellerCoordinates;
                if (sellerCoordinates != null)
                {
                    var distanceMatrix = _distanceMatrixCalculator.Calculate(_originCoordinates, sellerCoordinates);

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