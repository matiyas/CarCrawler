namespace CarCrawler.Configuration;

public interface IAppConfiguration
{
    T GetValue<T>(string key);
}