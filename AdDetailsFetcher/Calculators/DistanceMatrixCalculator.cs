using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcher.Calculators;

public class DistanceMatrixCalculator : IDistanceMatrixCalculator
{
    private readonly IDistanceMatrixCalculatorProvider _provider;

    public DistanceMatrixCalculator(IDistanceMatrixCalculatorProvider provider)
    {
        _provider = provider;
    }

    public async Task<DistanceMatrix?> Calculate(Point? origin, Point? destination)
    {
        if (origin is null || destination is null) return null;

        _provider.Origin = origin;
        _provider.Destination = destination;

        return await _provider.GetDistanceMatrix();
    }
}