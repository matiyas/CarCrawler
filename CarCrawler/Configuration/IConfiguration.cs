namespace CarCrawler.Configuration;

public interface IConfiguration
{
    T GetValue<T>(string key);
}