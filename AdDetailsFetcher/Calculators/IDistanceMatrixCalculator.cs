using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcher.Calculators
{
    public interface IDistanceMatrixCalculator
    {
        Task<DistanceMatrix?> Calculate(Point origin, Point destination);
    }
}