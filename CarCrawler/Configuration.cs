using Microsoft.Extensions.Configuration;

namespace CarCrawler;

internal static class Configuration
{
    private static readonly IConfiguration _configuration;

    static Configuration()
    {
        if (_configuration != null) return;

        _configuration = new ConfigurationBuilder()
           .AddJsonFile("config/car_crawler.json")
           .Build();
    }

    public static IConfiguration Get => _configuration;
}