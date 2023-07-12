using NetTopologySuite.Geometries;

namespace CarCrawler.Calculators;

public class DistanceMatrixCalculator : IDistanceMatrixCalculator
{
    private readonly IDistanceMatrixCalculatorProvider _provider;

    public DistanceMatrixCalculator(IDistanceMatrixCalculatorProvider provider)
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