using System.Numerics;

namespace CarCrawler.Models;

internal class AdDetails
{
    public enum Fuel
    {
        Petrol = 0,
        PetrolAndCNG = 1,
        PetrolAndLPG = 2,
        Diesel = 3,
        Electric = 4,
        Ethanol = 5,
        Hybrid = 6,
        Hydrogen = 7
    }

    public string? Id { get; set; }
    public string? Brand { get; set; }
    public string? Description { get; set; }
    public Fuel? FuelType { get; set; }
    public string? ISOCurrencySymbol { get; set; }
    public uint? MileageKilometers { get; set; }
    public string? Model { get; set; }
    public string? Name { get; set; }
    public decimal? Price { get; set; }
    public DateOnly? RegistrationDate { get; set; }
    public string? RegistrationNumber { get; set; }
    public Vector2? SellerCoordinates { get; set; }
    public IEnumerable<string>? SellerPhones { get; set; }
    public Uri? Url { get; set; }
    public string? VIN { get; set; }
    public string? Year { get; set; }
}