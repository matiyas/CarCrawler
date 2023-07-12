using NetTopologySuite.Geometries;

namespace CarCrawler.Calculators
{
    public interface IDistanceMatrixCalculator
    {
        DistanceMatrix? Calculate(Point origin, Point destination);
    }
}