using CarCrawler.Services.Calculators.Providers.Interfaces;
using NetTopologySuite.Geometries;

namespace CarCrawler.Services.Calculators;

internal class DistanceMatrixCalculator
{
    private readonly IDistanceMatrixProvider _provider;

    public DistanceMatrixCalculator(IDistanceMatrixProvider provider)
    {
        _provider = provider;
    }

    public DistanceMatrix? Calculate(Point origin, Point destination)
    {
        _provider.Origin = origin;
        _provider.Destination = destination;

        return _provider.GetDistanceMatrix();
    }
}