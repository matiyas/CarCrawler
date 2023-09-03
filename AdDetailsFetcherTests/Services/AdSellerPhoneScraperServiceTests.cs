using AdDetailsFetcher.Models;
using AdDetailsFetcher.Services;
using AppLogger;
using Moq;
using RichardSzalay.MockHttp;
using System.Net;

namespace AdDetailsFetcherTests.Services;

public class AdSellerPhoneScraperServiceTests
{
    private readonly AdDetails _adDetails;
    private readonly Mock<IAppLogger> _loggerMock;
    private readonly string _offerId = "6FHN9W";
    private readonly MockHttpMessageHandler _httpHandlerMock;

    public AdSellerPhoneScraperServiceTests()
    {
        _adDetails = new AdDetails();
        _loggerMock = new Mock<IAppLogger>();
        _httpHandlerMock = new MockHttpMessageHandler();
    }

    [Fact]
    public async Task GetSellerPhonesFromOfferId_OfferIdIsNull_DoesNotSetPhonesInAdDetails()
    {
        // Arrange
        var scraper = new AdSellerPhoneScraperService(_adDetails);

        // Act
        await scraper.GetSellerPhonesFromOfferId(null);

        // Assert
        Assert.Null(_adDetails.SellerPhones);
    }

    [Fact]
    public async Task GetSellerPhonesFromOfferId_NotFound_DoesNotSetPhonesInAdDetails()
    {
        // Arrange
        SetupHttpResponse(HttpStatusCode.NotFound, "Offer not found");
        var scraper = new AdSellerPhoneScraperService(_adDetails, _loggerMock.Object, _httpHandlerMock.ToHttpClient());

        // Act
        await scraper.GetSellerPhonesFromOfferId(_offerId);

        // Assert
        _loggerMock.Verify(logger => logger.Log("An unexpected error occurred:"), Times.Once);
        _loggerMock.Verify(logger => logger.Log("Not Found"), Times.Once);
        Assert.Null(_adDetails.SellerPhones);
    }

    [Fact]
    public async Task GetSellerPhonesFromOfferId_HttpRequestException_DoesNotSetPhonesInAdDetails()
    {
        // Arrange
        _httpHandlerMock.Fallback.Throw(new HttpRequestException("Bad request"));
        var scraper = new AdSellerPhoneScraperService(_adDetails, _loggerMock.Object, _httpHandlerMock.ToHttpClient());

        // Act
        await scraper.GetSellerPhonesFromOfferId(_offerId);

        // Assert
        _loggerMock.Verify(logger => logger.Log("An unexpected error occurred:"), Times.Once);
        _loggerMock.Verify(logger => logger.Log("Bad request"), Times.Once);
        Assert.Null(_adDetails.SellerPhones);
    }

    [Fact]
    public async Task GetSellerPhonesFromOfferId_Success_SetPhonesInAdDetails()
    {
        // Arrange
        var response = File.ReadAllText("TestData/otomoto/all-phones.json");
        SetupHttpResponse(HttpStatusCode.OK, response);
        var scraper = new AdSellerPhoneScraperService(_adDetails, _httpHandlerMock.ToHttpClient());

        // Act
        await scraper.GetSellerPhonesFromOfferId(_offerId);

        // Assert
        Assert.Equal(new[] { "534666742" }, _adDetails.SellerPhones!);
    }

    private void SetupHttpResponse(HttpStatusCode statusCode, string? response = "")
    {
        var headers = new Dictionary<string, string>
        {
            { "Accept", "application/json" },
            { "User-Agent", "Mozilla/5.0" }
        };

        _httpHandlerMock
            .Expect(@$"https://www.otomoto.pl/ajax/misc/contact/all_phones/{_offerId}/")
            .WithHeaders(headers)
            .Respond(statusCode, "application/json", response);
    }
}