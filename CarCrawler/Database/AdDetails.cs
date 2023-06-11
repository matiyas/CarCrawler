﻿using NetTopologySuite.Geometries;

namespace CarCrawler.Database;

internal class AdDetails : BaseEntity
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

    public int Id { get; set; }
    public string? ExternalId { get; set; }
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
    public Point? SellerCoordinates { get; set; }
    public IEnumerable<string>? SellerPhones { get; set; }
    public Uri? Url { get; set; }
    public string? VIN { get; set; }
    public string? Year { get; set; }
    public TimeSpan? TravelDuration { get; set; }
    public int? TravelDistance { get; set; }
    public VehicleHistoryReport? VehicleHistoryReport { get; set; }

    public static IEnumerable<string> SpreadsheetColumns => new[]
    {
        "Id",
        "Brand",
        "Description",
        "FuelType",
        "ISOCurrencySymbol",
        "MileageKilometers",
        "Model",
        "Name",
        "Price",
        "RegistrationDate",
        "RegistrationNumber",
        "SellerCoordinates",
        "SellerPhones",
        "Url",
        "VIN",
        "Year",
        "TravelDuration",
        "TravelDistance"
    };
}