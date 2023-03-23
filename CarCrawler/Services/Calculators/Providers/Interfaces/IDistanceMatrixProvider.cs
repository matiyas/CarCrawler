using System.Numerics;

namespace CarCrawler.Services.Calculators.Providers.Interfaces;

internal interface IDistanceMatrixProvider
{
    Vector2 Origin { get; set; }
    Vector2 Destination { get; set; }

    DistanceMatrix? GetDistanceMatrix ();
}
