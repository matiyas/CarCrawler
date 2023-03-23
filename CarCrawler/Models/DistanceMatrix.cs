using System.Numerics;

namespace CarCrawler.Models;

internal class DistanceMatrix
{
    public Vector2 Origin { get; set; }
    public Vector2 Destination { get; set; }
    public int? DistanceMeters { get; set; }
    public TimeSpan? Duration { get; set; }
}
