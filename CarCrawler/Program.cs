global using CarCrawler.Configuration;
global using CarCrawler.Models;

namespace CarCrawler;

public class Program
{
    private static async Task Main()
    {
        var app = new App();
        await app.Run();

        app.Dispose();
    }
}