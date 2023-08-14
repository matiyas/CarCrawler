using NetTopologySuite.Geometries;
using AdDetailsFetcher.Models;

namespace AdDetailsFetcher.Calculators;

public interface IDistanceMatrixCalculatorProvider
{
    Point Origin { get; set; }
    Point Destination { get; set; }

    DistanceMatrix? GetDistanceMatrix();
}