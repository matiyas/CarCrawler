using NetTopologySuite.Geometries;

namespace CarCrawler.Services.Calculators.Providers.Interfaces;

internal interface IDistanceMatrixProvider
{
    Point Origin { get; set; }
    Point Destination { get; set; }

    DistanceMatrix? GetDistanceMatrix ();
}
