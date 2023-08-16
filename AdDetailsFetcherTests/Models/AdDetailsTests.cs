using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;

namespace AdDetailsFetcherTests.Models;

public class AdDetailsTests
{
    [Fact]
    public void AdDetails_SetProperties_PropertiesAreSetCorrectly()
    {
        // Arrange
        var adDetails = new AdDetails
        {
            // Act
            Id = "123",
            Brand = "Toyota",
            Description = "Lorem ipsum dolor sit amet",
            FuelType = Fuel.Petrol,
            ISOCurrencySymbol = "USD",
            MileageKilometers = 50000,
            Model = "Supra",
            Name = "Needle condition. The only one. Check!!!",
            Price = 15000,
            RegistrationDate = new DateOnly(2022, 1, 1),
            RegistrationNumber = "ABC123",
            SellerCoordinates = new Point(1.23, 4.56),
            SellerPhones = new[] { "123456789", "987654321" },
            Url = new Uri("https://example.com"),
            VIN = "1234567890",
            Year = "2021",
            TravelDuration = TimeSpan.FromHours(3),
            TravelDistance = 100
        };

        // Assert
        Assert.Equal("123", adDetails.Id);
        Assert.Equal("Toyota", adDetails.Brand);
        Assert.Equal("Lorem ipsum dolor sit amet", adDetails.Description);
        Assert.Equal(Fuel.Petrol, adDetails.FuelType);
        Assert.Equal("USD", adDetails.ISOCurrencySymbol);
        Assert.Equal(50000u, adDetails.MileageKilometers);
        Assert.Equal("Supra", adDetails.Model);
        Assert.Equal("Needle condition. The only one. Check!!!", adDetails.Name);
        Assert.Equal(15000m, adDetails.Price);
        Assert.Equal(new DateOnly(2022, 1, 1), adDetails.RegistrationDate);
        Assert.Equal("ABC123", adDetails.RegistrationNumber);
        Assert.Equal(new Point(1.23, 4.56), adDetails.SellerCoordinates);
        Assert.Equal(new[] { "123456789", "987654321" }, adDetails.SellerPhones);
        Assert.Equal(new Uri("https://example.com"), adDetails.Url);
        Assert.Equal("1234567890", adDetails.VIN);
        Assert.Equal("2021", adDetails.Year);
        Assert.Equal(TimeSpan.FromHours(3), adDetails.TravelDuration);
        Assert.Equal(100, adDetails.TravelDistance);
    }

    [Fact]
    public void AdDetails_WithNullProperties_PropertiesAreSetToDefaultValues()
    {
        // Arrange
        var adDetails = new AdDetails();

        // Assert
        Assert.Null(adDetails.Id);
        Assert.Null(adDetails.Brand);
        Assert.Null(adDetails.Description);
        Assert.Null(adDetails.FuelType);
        Assert.Null(adDetails.ISOCurrencySymbol);
        Assert.Null(adDetails.MileageKilometers);
        Assert.Null(adDetails.Model);
        Assert.Null(adDetails.Name);
        Assert.Null(adDetails.Price);
        Assert.Null(adDetails.RegistrationDate);
        Assert.Null(adDetails.RegistrationNumber);
        Assert.Null(adDetails.SellerCoordinates);
        Assert.Null(adDetails.SellerPhones);
        Assert.Null(adDetails.Url);
        Assert.Null(adDetails.VIN);
        Assert.Null(adDetails.Year);
        Assert.Null(adDetails.TravelDuration);
        Assert.Null(adDetails.TravelDistance);
    }
}