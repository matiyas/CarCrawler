using NetTopologySuite.Geometries;

namespace AdDetailsFetcher.Models;

public class DistanceMatrix
{
    public Point Origin { get; set; } = Point.Empty;
    public Point Destination { get; set; } = Point.Empty;
    public int? DistanceMeters { get; set; }
    public TimeSpan? Duration { get; set; }
}