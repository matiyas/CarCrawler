using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcher.Calculators;

public interface IDistanceMatrixCalculatorProvider
{
    Point Origin { get; set; }
    Point Destination { get; set; }

    DistanceMatrix? GetDistanceMatrix();
}