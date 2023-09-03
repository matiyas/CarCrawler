using AdDetailsFetcher.Calculators;
using AdDetailsFetcher.Models;
using Moq;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcherTests;

public class DistanceMatrixCalculatorTests
{
    private readonly DistanceMatrix _distanceMatrix = Mock.Of<DistanceMatrix>();

    private IDistanceMatrixCalculatorProvider DistanceMatrixCalculatorProviderMock
    {
        get
        {
            return Mock.Of<IDistanceMatrixCalculatorProvider>(provider =>
                provider.GetDistanceMatrix() == Task.FromResult(_distanceMatrix));
        }
    }

    private DistanceMatrixCalculator DistanceMatrixCalculator => new(DistanceMatrixCalculatorProviderMock);

    [Fact]
    public async Task Calculate_OriginAndDestinationNotNull_ReturnsDistanceMatrix()
    {
        Point? origin = new(0, 0);
        Point? destination = new(0, 0);

        var result = await DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.NotNull(result);
        Assert.Equal(_distanceMatrix, result);
    }

    [Fact]
    public async Task Calculate_OriginIsNullAndDestinationNotNull_ReturnsNull()
    {
        Point? origin = null;
        Point? destination = new(0, 0);

        var result = await DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.Null(result);
    }

    [Fact]
    public async Task Calculate_OriginIsNotNullAndDestinationIsNull_ReturnsNull()
    {
        Point? origin = new(0, 0);
        Point? destination = null;

        var result = await DistanceMatrixCalculator.Calculate(origin, destination);

        Assert.Null(result);
    }
}