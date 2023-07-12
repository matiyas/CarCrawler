global using CarCrawler.Models;
global using CarCrawler.Configuration;

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