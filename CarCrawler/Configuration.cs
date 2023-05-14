using Newtonsoft.Json;

namespace CarCrawler;

internal class Configuration
{
    public Uri OffertUrl { get; set; } = null!;
    public string SpreadsheetId { get; set; } = null!;
    public string SpreadsheetName { get; set; } = null!;
    public float OriginCoordsLon { get; set; }
    public float OriginCoordsLat { get; set; }

    public static Configuration Read ()
    {
        var configPath = "config/car_crawler.json";
        var jsonString = File.ReadAllText(configPath);

        return JsonConvert.DeserializeObject<Configuration>(jsonString)!;
    }
}
