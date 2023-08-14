using AdDetailsFetcher.Calculators;
using AdDetailsFetcher.Models;
using Moq;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcherTests;

public class DistanceMatrixCalculatorTests
{
    private readonly DistanceMatrix _distanceMatrix = Mock.Of<DistanceMatrix>();

    private IDistanceMatrixCalculatorProvider DistanceMatrixCalculatorProviderMock =>
            Mock.Of<IDistanceMatrixCalculatorProvider>(provider => provider.GetDistanceMatrix() == _distanceMatrix);

    private DistanceMatrixCalculator DistanceMatrixCalculator => new(DistanceMatrixCalculatorProviderMock);

    [Fact]
    public void Calculate_OriginAndDestinationNotNull_ReturnsDistanceMatrix()
    {
        Point? origin = new(0, 0);
        Point? destination = new(0, 0);

        var result = DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.NotNull(result);
        Assert.Equal(_distanceMatrix, result);
    }

    [Fact]
    public void Calculate_OriginIsNullAndDestinationNotNull_ReturnsNull()
    {
        Point? origin = null;
        Point? destination = new(0, 0);

        var result = DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.Null(result);
    }

    [Fact]
    public void Calculate_OriginIsNotNullAndDestinationIsNull_ReturnsNull()
    {
        Point? origin = new(0, 0);
        Point? destination = null;

        var result = DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.Null(result);
    }
}