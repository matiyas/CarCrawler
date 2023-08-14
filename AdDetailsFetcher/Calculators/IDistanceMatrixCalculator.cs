using NetTopologySuite.Geometries;
using AdDetailsFetcher.Models;

namespace AdDetailsFetcher.Calculators
{
    public interface IDistanceMatrixCalculator
    {
        DistanceMatrix? Calculate(Point origin, Point destination);
    }
}