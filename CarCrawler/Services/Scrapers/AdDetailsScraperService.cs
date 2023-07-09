using CarCrawler.Database;
using HtmlAgilityPack;
using ISO._4217;
using NetTopologySuite.Geometries;
using System.Globalization;
using System.Text.RegularExpressions;
using static CarCrawler.Database.AdDetails;

namespace CarCrawler.Services.Scrapers;

public class AdDetailsScraperService
{
    private readonly AdDetails _adDetails;
    private readonly Uri _adLink;

    private delegate bool ParserFunc<T>(string arg, out T? res);

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
        var priceNodeXPath = $@"{summaryNodeXPath}//div[contains(@class, ""offer-price"")]";

        #endregion XPaths

        #region Nodes

        var offerId = GetOfferIdFromUrl();
        var htmlDocNode = LoadHtmlDocNode();
        var summaryNode = htmlDocNode.SelectSingleNode(summaryNodeXPath);
        var priceNode = htmlDocNode.SelectSingleNode(priceNodeXPath);
        var parametersNodes = htmlDocNode.SelectNodes(parametersNodeXPath);
        var descriptionNode = htmlDocNode.SelectSingleNode(descriptionNodeXPath);

        #endregion Nodes

        #region Assignments

        _adDetails.Url = _adLink;
        GetIdFromHtmlDocNode(htmlDocNode);
        GetDetailsFromSummaryNode(summaryNode);
        GetDetailsFromPriceNode(priceNode);
        GetDetailsFromParametersNodes(parametersNodes);
        GetDetailsFromDescriptionNode(descriptionNode);
        GetVinFromHtmlDocNode(htmlDocNode);
        GetSellerCoordinatesFromHtmlDocNode(htmlDocNode);
        GetSellerPhonesFromOfferId(offerId);

        #endregion Assignments

        return _adDetails;
    }

    private void GetSellerPhonesFromOfferId(string? offerId)
    {
        var service = new AdSellerPhoneScraperService(_adDetails);
        service.GetSellerPhonesFromOfferId(offerId);
    }

    private void GetIdFromHtmlDocNode(HtmlNode htmlDocNode)
    {
        var adIdNodeXPath = @"//span[@id=""ad_id""]";
        var adIdNode = htmlDocNode.SelectSingleNode(adIdNodeXPath);
        var id = adIdNode?.InnerText?.Trim();

        _adDetails.ExternalId = id;
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

        _adDetails.SellerCoordinates = new Point(longitude, latitude);
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
        var priceString = priceNode.GetAttributeValue("data-price", "");
        priceString = Regex.Replace(priceString, @"\s+", "");

        if (string.IsNullOrWhiteSpace(priceString) || !decimal.TryParse(priceString, out var price))
        {
            return null;
        }

        return price;
    }

    private static void SetAdDetailsParamIfValid<T>(string? valueString, ParserFunc<T> parser, Action<T> setter)
    {
        if (!string.IsNullOrWhiteSpace(valueString) && parser(valueString, out T? parsedValue))
        {
            setter(parsedValue!);
        }
    }

    private static bool TryParseDateOnly(string dateString, out DateOnly result)
    {
        if (!DateOnly.TryParseExact(dateString, "dd/MM/yyyy", out var date))
        {
            return false;
        }
        result = date;

        return true;
    }

    private static bool TryParseFuelType(string fuelTypeString, out Fuel? result)
    {
        result = fuelTypeString?.ToLower() switch
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

        return result is not null;
    }

    private static bool TryParseMileageKilometers(string mileageKilometersString, out uint? result)
    {
        result = null;
        mileageKilometersString = Regex.Replace(mileageKilometersString, @"[^\d]", "");

        if (uint.TryParse(mileageKilometersString, out var mileageKilometers))
            result = mileageKilometers;

        return result is not null;
    }

    private void GetDetailsFromDescriptionNode(HtmlNode? descriptionNode)
    {
        _adDetails.Description = descriptionNode?.InnerText?.Trim();
    }

    private void GetDetailsFromParametersNodes(HtmlNodeCollection? paramsNodes)
    {
        if (paramsNodes == null) { return; }

        var paramsDict = GetParametersDictFromParametersNodes(paramsNodes);
        if (paramsDict is null) { return; }

        _adDetails.Brand = GetValueFromParamsDict(paramsDict, "Marka pojazdu");
        _adDetails.Model = GetValueFromParamsDict(paramsDict, "Model pojazdu");
        _adDetails.Year = GetValueFromParamsDict(paramsDict, "Rok produkcji");
        _adDetails.RegistrationNumber = GetValueFromParamsDict(paramsDict, "Data pierwszej rejestracji w historii pojazdu");

        SetAdDetailsParamIfValid<DateOnly>(
            GetValueFromParamsDict(paramsDict, "Numer rejestracyjny pojazdu"),
            TryParseDateOnly,
            value => _adDetails.RegistrationDate = value);

        SetAdDetailsParamIfValid<Fuel?>(
            GetValueFromParamsDict(paramsDict, "Rodzaj paliwa"),
            TryParseFuelType,
            value => _adDetails.FuelType = value);

        SetAdDetailsParamIfValid<uint?>(
            GetValueFromParamsDict(paramsDict, "Przebieg"),
            TryParseMileageKilometers,
            value => _adDetails.MileageKilometers = value);
    }

    private static string? GetValueFromParamsDict(Dictionary<string, string?> dict, string key)
    {
        dict.TryGetValue(key, out var result);
        return result;
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

    private string? GetOfferIdFromUrl()
    {
        var idMatch = Regex.Match(_adLink.ToString(), @"-ID(?<id>\w+)\.html");
        if (!idMatch.Success) return null;

        return idMatch.Groups["id"].ToString();
    }

    private HtmlNode LoadHtmlDocNode()
    {
        var web = new HtmlWeb();
        var doc = web.Load(_adLink);
        var node = doc.DocumentNode;

        return node;
    }
}