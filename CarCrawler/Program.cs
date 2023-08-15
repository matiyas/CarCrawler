global using CarCrawler.Configuration;
global using CarCrawler.Models;

namespace CarCrawler;

public class Program
{
    private static void Main()
    {
        var app = new App();
        app.Run();

        app.Dispose();
    }
}