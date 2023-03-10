using HtmlAgilityPack;
using ISO._4217;
using System.Globalization;
using System.Text.RegularExpressions;
using static CarCrawler.Models.AdDetails;

namespace CarCrawler.Services.Scrapers;

internal class AdDetailsScraperService
{
    private readonly AdDetails _adDetails;
    private readonly Uri _adLink;

    public AdDetailsScraperService(Uri adLink)
    {
        _adLink = adLink;
        _adDetails = new AdDetails();
    }

    public AdDetails? Call()
    {
        #region XPaths
        var descriptionNodeXPath = $@"//main//div[contains(@class, ""offer-description__description"")]";
        var parametersNodeXPath = $@"//main//div[contains(@class, ""parametersArea"")]//li[contains(@class, ""offer-params__item"")]";
        var summaryNodeXPath = @"//main//div[contains(@class, ""offer-summary"")]";
        var priceNodeXPath = $@"{summaryNodeXPath}//div[contains(@class, ""price-wrapper"")]//div[contains(@class, ""offer-price"")]";
        #endregion

        #region Nodes
        var htmlDocNode = LoadHtmlDocNode();
        var summaryNode = htmlDocNode.SelectSingleNode(summaryNodeXPath);
        var priceNode = htmlDocNode.SelectSingleNode(priceNodeXPath);
        var parametersNodes = htmlDocNode.SelectNodes(parametersNodeXPath);
        var descriptionNode = htmlDocNode.SelectSingleNode(descriptionNodeXPath);
        #endregion

        #region Assignments
        _adDetails.Url = _adLink;
        GetIdFromHtmlDocNode(htmlDocNode);
        GetDetailsFromSummaryNode(summaryNode);
        GetDetailsFromPriceNode(priceNode);
        GetDetailsFromParametersNodes(parametersNodes);
        GetDetailsFromDescriptionNode(descriptionNode);
        GetVinFromHtmlDocNode(htmlDocNode);
        GetSellerCoordinatesFromHtmlDocNode(htmlDocNode);
        GetSellerPhonesFromHtmlDocNode(htmlDocNode);
        #endregion

        return _adDetails;
    }

    private void GetIdFromHtmlDocNode(HtmlNode htmlDocNode)
    {
        var adIdNodeXPath = @"//span[@id=""ad_id""]";
        var adIdNode = htmlDocNode.SelectSingleNode(adIdNodeXPath);
        var id = adIdNode?.InnerText?.Trim();

        _adDetails.Id = id;
    }

    private void GetSellerPhonesFromHtmlDocNode(HtmlNode htmlDocNode)
    {
        var sellerPhonesNodesXPath = @"//span[contains(@class, ""seller-phones__number"")]";
        var sellerPhonesNodes = htmlDocNode.SelectNodes(sellerPhonesNodesXPath);
        if (sellerPhonesNodes == null)
        {
            return;
        }

        var sellerPhones = sellerPhonesNodes.Select(node => node.InnerText?.Trim());
        sellerPhones = sellerPhones.Where(phoneString => 
        {
            return !string.IsNullOrWhiteSpace(phoneString) && Regex.IsMatch(phoneString, @"(\d.*){9,}");
        });

        _adDetails.SellerPhones = sellerPhones.Select(phone => phone!);
    }

    private void GetSellerCoordinatesFromHtmlDocNode(HtmlNode htmlDocNode)
    {
        var adMapDataNodeXPath = @"//input[contains(@id, ""adMapData"")]";
        var adMapDataNode = htmlDocNode.SelectSingleNode(adMapDataNodeXPath);
        if (adMapDataNode == null)
        {
            return;
        }

        var longtitudeString = adMapDataNode.GetAttributeValue("data-map-lon", null);
        var latitudeString = adMapDataNode.GetAttributeValue("data-map-lat", null);

        if (!float.TryParse(longtitudeString, NumberStyles.Any, CultureInfo.InvariantCulture, out var latitude) || 
            !float.TryParse(latitudeString, NumberStyles.Any, CultureInfo.InvariantCulture, out var longitude))
        {
            return;
        }

        _adDetails.SellerCoordinates = new System.Numerics.Vector2(longitude, latitude);
    }

    private static string? GetAdISOCurrencySymbolFromAdSummaryNode(HtmlNode priceNode)
    {
        var currencyNodeXPath = @".//span[contains(@class, ""offer-price__currency"")]";
        var currencyString = priceNode.SelectSingleNode(currencyNodeXPath)?.InnerText?.Trim();

        if (currencyString == null)
        {
            return null;
        }

        return CurrencyCodesResolver.GetCurrenciesByCode(currencyString).FirstOrDefault()?.Code;
    }

    private static string? GetAdNameFromAdSummaryNode(HtmlNode summaryNode)
    {
        var titleNodeXPath = @".//span[contains(@class, ""offer-title"")]";
        var titleNode = summaryNode.SelectSingleNode(titleNodeXPath);

        return titleNode?.InnerText?.Trim();
    }

    private static decimal? GetAdPriceFromAdPriceNode(HtmlNode priceNode)
    {
        var priceString = priceNode.GetAttributeValue("data-price", null)?.Trim();

        if (string.IsNullOrWhiteSpace(priceString) || !decimal.TryParse(priceString, out decimal price))
        {
            return null;
        }


        return price;
    }

    private static DateOnly? TryParseDateOnly(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString) || !DateOnly.TryParseExact(dateString, "dd/MM/yyyy", out var date))
        {
            return null;
        }

        return date;
    }

    private static Fuel? TryParseFuelType(string? fuelTypeString)
    {
        return fuelTypeString?.ToLower() switch
        {
            "benzyna" => Fuel.Petrol,
            "benzyna+lpg" => Fuel.PetrolAndLPG,
            "benzyna+cng" => Fuel.PetrolAndCNG,
            "diesel" => Fuel.Diesel,
            "elektryczny" => Fuel.Electric,
            "etanol" => Fuel.Ethanol,
            "hybryda" => Fuel.Hybrid,
            "wodór" => Fuel.Hydrogen,
            _ => null
        };
    }

    private static uint? TryParseMileageKilometers(string? mileageKilometersString)
    {
        if (string.IsNullOrWhiteSpace(mileageKilometersString)) { return null; }

        mileageKilometersString = Regex.Replace(mileageKilometersString, @"[^\d]", "");
        if (!uint.TryParse(mileageKilometersString, out var mileageKilometers)) { return null; }

        return mileageKilometers;
    }

    private void GetDetailsFromDescriptionNode(HtmlNode? descriptionNode)
    {
        _adDetails.Description = descriptionNode?.InnerText?.Trim();
    }

    private void GetDetailsFromParametersNodes(HtmlNodeCollection? parametersNodes)
    {
        if (parametersNodes == null) { return; }

        var parametersDict = GetParametersDictFromParametersNodes(parametersNodes);

        parametersDict.TryGetValue("Marka pojazdu", out var brand);
        parametersDict.TryGetValue("Model pojazdu", out var model);
        parametersDict.TryGetValue("Rok produkcji", out var year);
        parametersDict.TryGetValue("Data pierwszej rejestracji w historii pojazdu", out var registrationDate);
        parametersDict.TryGetValue("Numer rejestracyjny pojazdu", out var registrationNumber);
        parametersDict.TryGetValue("Rodzaj paliwa", out var fuelType);
        parametersDict.TryGetValue("Przebieg", out var mileageKilometers);

        _adDetails.Brand = brand;
        _adDetails.Model = model;
        _adDetails.Year = year;
        _adDetails.RegistrationDate = TryParseDateOnly(registrationDate);
        _adDetails.RegistrationNumber = registrationNumber;
        _adDetails.FuelType = TryParseFuelType(fuelType);
        _adDetails.MileageKilometers = TryParseMileageKilometers(mileageKilometers);
    }

    private void GetDetailsFromPriceNode(HtmlNode? priceNode)
    {
        if (priceNode == null) 
        { 
            return; 
        }

        _adDetails.Price = GetAdPriceFromAdPriceNode(priceNode);
    }

    private void GetDetailsFromSummaryNode(HtmlNode summaryNode)
    {
        if (summaryNode == null) 
        { 
            return; 
        }

        _adDetails.Name = GetAdNameFromAdSummaryNode(summaryNode);
        _adDetails.ISOCurrencySymbol = GetAdISOCurrencySymbolFromAdSummaryNode(summaryNode);
    }

    private Dictionary<string, string?> GetParametersDictFromParametersNodes(HtmlNodeCollection parametersNodes)
    {
        var parameters = parametersNodes.Select(parameter =>
        {
            var label = parameter.SelectSingleNode(@".//span[contains(@class, ""offer-params__label"")]")?.InnerText?.Trim();
            var value = parameter.SelectSingleNode(@".//div[contains(@class, ""offer-params__value"")]")?.InnerText?.Trim();

            return (Label: label, Value: value);
        });

        parameters = parameters.Where(parameter => parameter.Label != null);

        return parameters.ToDictionary(tuple => tuple.Label!, tuple => tuple.Value);
    }

    private void GetVinFromHtmlDocNode(HtmlNode htmlDocNode)
    {
        var innerHtml = htmlDocNode.InnerHtml;
        var vinRegex = @"GPT\.targeting\s*=\s*\{.*""vin""\s*:\s*\[""([a-zA-Z0-9]+)";
        var vinMatch = Regex.Match(innerHtml, vinRegex);

        if (vinMatch.Success)
        {
            _adDetails.VIN = vinMatch.Result("$1");
        }
    }
    private HtmlNode LoadHtmlDocNode()
    {
        var web = new HtmlWeb();
        var doc = web.Load(_adLink);
        var node = doc.DocumentNode;

        return node;
    }
}