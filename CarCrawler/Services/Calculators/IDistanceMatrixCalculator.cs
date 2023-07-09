using NetTopologySuite.Geometries;

namespace CarCrawler.Services.Calculators
{
    public interface IDistanceMatrixCalculator
    {
        DistanceMatrix? Calculate(Point origin, Point destination);
    }
}