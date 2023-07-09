using Microsoft.Extensions.Configuration;

namespace CarCrawler.Configuration;

public class AppConfiguration : IAppConfiguration
{
    private readonly IConfiguration _configuration;

    public AppConfiguration()
    {
        _configuration =
            new ConfigurationBuilder()
                .AddJsonFile("Configuration/car_crawler.json")
                .Build();
    }

    public T GetValue<T>(string key) => _configuration.GetValue<T>(key);
}