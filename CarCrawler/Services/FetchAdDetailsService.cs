using CarCrawler.Database;
using CarCrawler.Services.Calculators;
using CarCrawler.Services.Scrapers;
using NetTopologySuite.Geometries;

namespace CarCrawler.Services;

public class FetchAdDetailsService
{
    private readonly Uri _adsListLink;
    private readonly IDistanceMatrixCalculator _distanceMatrixCalculator;
    private readonly Point _originCoordinates;
    private readonly IAppLogger _logger;

    public FetchAdDetailsService(
        Uri adsListLink,
        IDistanceMatrixCalculator distanceMatrixCalculator,
        Point originCoords)
    {
        _adsListLink = adsListLink;
        _distanceMatrixCalculator = distanceMatrixCalculator;
        _originCoordinates = originCoords;
    }

    public FetchAdDetailsService(
        Uri adsListLink,
        IDistanceMatrixCalculator distanceMatrixCalculator,
        Point originCoords,
        IAppLogger logger) : this(adsListLink, distanceMatrixCalculator, originCoords)
    {
        _logger = logger;
    }

    public IEnumerable<AdDetails> Execute()
    {
        var adListLinksScraperService = new AdListLinksScraperService(_adsListLink);

        foreach (var pageLinks in adListLinksScraperService.GetLinksFromPages())
        {
            var pageLinksArray = pageLinks.ToArray();

            for (var i = 0; i < pageLinksArray.Length; i++)
            {
                _logger?.Log($"Processing link {i + 1}/{pageLinksArray.Length}...");

                var newAdDetails = new AdDetailsScraperService(pageLinksArray[i]).Call();
                if (newAdDetails == null) continue;

                CalculateDistanceFromSeller(newAdDetails);

                yield return newAdDetails!;
            }
        }
    }

    private void CalculateDistanceFromSeller(AdDetails? newAdDetails)
    {
        var sellerCoordinates = newAdDetails?.SellerCoordinates;
        if (sellerCoordinates == null) return;

        var distanceMatrix = _distanceMatrixCalculator.Calculate(_originCoordinates, sellerCoordinates);
        if (distanceMatrix == null) return;

        newAdDetails!.TravelDuration = distanceMatrix.Duration;
        newAdDetails!.TravelDistance = distanceMatrix.DistanceMeters;
    }
}