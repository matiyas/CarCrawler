using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcherTests.Models;

public class DistanceMatrixTests
{
    [Fact]
    public void DistanceMatrix_SetProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var distanceMatrix = new DistanceMatrix
        {
            // Act
            Origin = new Point(1.23, 4.56),
            Destination = new Point(7.89, 10.11),
            DistanceMeters = 2137,
            Duration = TimeSpan.FromHours(3)
        };

        // Assert
        Assert.Equal(new Point(1.23, 4.56), distanceMatrix.Origin);
        Assert.Equal(new Point(7.89, 10.11), distanceMatrix.Destination);
        Assert.Equal(2137, distanceMatrix.DistanceMeters);
        Assert.Equal(TimeSpan.FromHours(3), distanceMatrix.Duration);
    }

    [Fact]
    public void DistanceMatrix_WithoutSettingProperties_PropertiesHaveCorrectDefaultValues()
    {
        // Arrange
        var distanceMatrix = new DistanceMatrix();

        // Assert
        Assert.Equal(Point.Empty, distanceMatrix.Origin);
        Assert.Equal(Point.Empty, distanceMatrix.Destination);
        Assert.Null(distanceMatrix.DistanceMeters);
        Assert.Null(distanceMatrix.Duration);
    }
}