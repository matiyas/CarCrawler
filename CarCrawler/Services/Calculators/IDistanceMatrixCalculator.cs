using NetTopologySuite.Geometries;

namespace CarCrawler.Services.Calculators
{
    internal interface IDistanceMatrixCalculator
    {
        DistanceMatrix? Calculate(Point origin, Point destination);
    }
}