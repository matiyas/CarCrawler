using NetTopologySuite.Geometries;

namespace CarCrawler.Models;

public class DistanceMatrix
{
    public Point Origin { get; set; }
    public Point Destination { get; set; }
    public int? DistanceMeters { get; set; }
    public TimeSpan? Duration { get; set; }
}