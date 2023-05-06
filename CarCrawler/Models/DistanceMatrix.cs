using NetTopologySuite.Geometries;

namespace CarCrawler.Models;

internal class DistanceMatrix
{
    public Point Origin { get; set; }
    public Point Destination { get; set; }
    public int? DistanceMeters { get; set; }
    public TimeSpan? Duration { get; set; }
}
