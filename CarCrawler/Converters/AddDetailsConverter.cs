namespace CarCrawler.Converters;

internal static class AddDetailsConverter
{
    public static AdDetails Convert(AdDetailsFetcher.Models.AdDetails adDetailsSrc)
    {
        return new AdDetails
        {
            ExternalId = adDetailsSrc.Id,
            Brand = adDetailsSrc.Brand,
            Description = adDetailsSrc.Description,
            FuelType = (Fuel?)adDetailsSrc.FuelType,
            ISOCurrencySymbol = adDetailsSrc.ISOCurrencySymbol,
            MileageKilometers = adDetailsSrc.MileageKilometers,
            Model = adDetailsSrc.Model,
            Name = adDetailsSrc.Name,
            Price = adDetailsSrc.Price,
            RegistrationDate = adDetailsSrc.RegistrationDate,
            RegistrationNumber = adDetailsSrc.RegistrationNumber,
            SellerCoordinates = adDetailsSrc.SellerCoordinates,
            SellerPhones = adDetailsSrc.SellerPhones,
            Url = adDetailsSrc.Url,
            VIN = adDetailsSrc.VIN,
            Year = adDetailsSrc.Year,
            TravelDuration = adDetailsSrc.TravelDuration,
            TravelDistance = adDetailsSrc.TravelDistance,
            VehicleHistoryReport = null
        };
    }
}