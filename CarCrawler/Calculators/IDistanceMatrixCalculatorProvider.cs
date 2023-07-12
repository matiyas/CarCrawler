using NetTopologySuite.Geometries;

namespace CarCrawler.Calculators;

public interface IDistanceMatrixCalculatorProvider
{
    Point Origin { get; set; }
    Point Destination { get; set; }

    DistanceMatrix? GetDistanceMatrix();
}