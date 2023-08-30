using AdDetailsFetcher.Models;
using AdDetailsFetcher.Services;
using HtmlAgilityPack;
using Moq;
using Moq.Protected;
using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;

namespace AdDetailsFetcherTests.Services;

public class AdDetailsScraperServiceTests
{
    [Fact]
    public void Call_Success_ReturnsAdDetails()
    {
        // Arrange
        var url = new Uri("https://otomoto.pl/osobowe/oferta/foo-bar-baz-ID6FBfqN.html");
        var htmlDocument = new HtmlDocument();
        htmlDocument.Load("TestData/otomoto/offer.html");
        var scraperMock = new Mock<AdDetailsScraperService>(url);
        scraperMock
            .Protected()
            .Setup<HtmlNode>("LoadHtmlDocNode")
            .Returns(htmlDocument.DocumentNode);

        // Act
        var result = scraperMock.Object.Call();

        // Assert
        Assert.IsType<AdDetails>(result);
        Assert.Equal("Honda", result.Brand);
        Assert.Equal("Sprzedam Hondę Civic IX generacji.", result.Description!);
        Assert.Equal(Fuel.Petrol, result.FuelType);
        Assert.Equal("PLN", result.ISOCurrencySymbol);
        Assert.Equal(28_000u, result.MileageKilometers);
        Assert.Equal("Civic", result.Model);
        Assert.Equal("Honda Civic 1.8 Sport", Regex.Replace(result.Name!, @"[\r\n]", ""));
        Assert.Equal(60000, result.Price);
        Assert.Equal(new DateOnly(2015, 3, 20), result.RegistrationDate);
        Assert.Equal("CTR6PR9", result.RegistrationNumber);
        Assert.Equal(new Point(53.18478012084961, 18.606000900268555), result.SellerCoordinates);
        Assert.Equal(new[] { "531664970" }, result.SellerPhones!);
        Assert.Equal("https://otomoto.pl/osobowe/oferta/foo-bar-baz-ID6FBfqN.html", result.Url!.ToString());
        Assert.Equal("SHHFK2760EU011595", result.VIN);
        Assert.Equal("2014", result.Year);
        Assert.Null(result.TravelDuration);
        Assert.Null(result.TravelDistance);
    }
}