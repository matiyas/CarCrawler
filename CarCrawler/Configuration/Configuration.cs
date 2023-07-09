using Microsoft.Extensions.Configuration;

using MsConfiguration = Microsoft.Extensions.Configuration;

namespace CarCrawler.Configuration;

public class Configuration : IConfiguration
{
    private readonly MsConfiguration.IConfiguration _configuration;

    public Configuration()
    {
        _configuration =
            new ConfigurationBuilder()
                .AddJsonFile("Configuration/car_crawler.json")
                .Build();
    }

    public T GetValue<T>(string key) => _configuration.GetValue<T>(key);
}