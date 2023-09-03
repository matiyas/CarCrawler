using AdDetailsFetcher.Calculators;
using AdDetailsFetcher.Models;
using AppLogger;
using Moq;
using Moq.Protected;
using NetTopologySuite.Geometries;
using RichardSzalay.MockHttp;
using System.Globalization;
using System.Net;

namespace AdDetailsFetcherTests.Calculators;

public class GoogleDistanceMatrixCalculatorProviderTests
{
    private readonly Mock<IAppLogger> _loggerMock;
    private readonly Mock<GoogleDistanceMatrixCalculatorProvider> _providerMock;
    private readonly MockHttpMessageHandler _httpHandlerMock;

    public GoogleDistanceMatrixCalculatorProviderTests()
    {
        _loggerMock = new Mock<IAppLogger>();
        _providerMock = new Mock<GoogleDistanceMatrixCalculatorProvider>(_loggerMock.Object);
        _httpHandlerMock = new MockHttpMessageHandler();
        _providerMock
            .Protected()
            .Setup<HttpClient>("GetHttpClient")
            .Returns(_httpHandlerMock.ToHttpClient);

        CultureInfo culture = new("en-US");
        CultureInfo.DefaultThreadCurrentCulture = culture;
    }

    [Fact]
    public async Task GetDistanceMatrix_HttpRequestExceptionWhileSendingRequest_ReturnsNull()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.InternalServerError);

        // Act
        var result = await _providerMock.Object.GetDistanceMatrix();

        // Assert
        Assert.Null(result);
        _loggerMock.Verify(logger => logger.Log("An unexpected error occurred:"), Times.Once);
        _loggerMock.Verify(logger => logger.Log("Internal Server Error"), Times.Once);
    }

    [Fact]
    public async Task GetDistanceMatrix_SuccessResponse_ReturnsDistanceMatrix()
    {
        // Arrange
        var responseJson = File.ReadAllText("TestData/google/maps/success_response.json");
        var queryString =
            new Dictionary<string, string>
            {
                { "origins", "48.858475952373254,2.294449110426952" },
                { "destinations", "48.855207936096825,2.279740804223466" },
                { "key", "123123123" }
            };
        SetupHttpResponse(HttpStatusCode.OK, responseJson, queryString);

        var provider = _providerMock.Object;
        provider.Origin = new Point(48.858475952373254, 2.294449110426952);
        provider.Destination = new Point(48.855207936096825, 2.279740804223466);

        // Act
        var result = await provider.GetDistanceMatrix();

        // Assert
        Assert.IsType<DistanceMatrix>(result);
        Assert.Equal(provider.Origin, result.Origin);
        Assert.Equal(provider.Destination, result.Destination);
        Assert.Equal(1329, result.DistanceMeters);
        Assert.Equal("00:05:09", result.Duration.ToString());
        _loggerMock.Verify(logger => logger.Log(It.IsAny<string>()), Times.Never);
    }

    private void SetupHttpResponse(
        HttpStatusCode statusCode,
        string? response = "",
        Dictionary<string, string>? queryString = null)
    {
        queryString ??= new Dictionary<string, string>();
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "Mozilla/5.0" }
        };

        _httpHandlerMock
            .Expect("https://maps.googleapis.com/maps/api/distancematrix/json")
            .WithQueryString(queryString)
            .WithHeaders(headers)
            .Respond(statusCode, "application/json", response);
    }
}