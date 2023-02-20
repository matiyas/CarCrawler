using CarCrawler.Models;
using CarCrawler.Services.Scrapers;

//var adListLinksScraperService = new AdListLinksScraperService(new Uri(@"https://www.otomoto.pl/osobowe/honda/civic"));
var adListLinksScraperService = new AdListLinksScraperService(new Uri($@"https://www.otomoto.pl/osobowe/bmw/seria-3?search%5Bfilter_enum_generation%5D=gen-e36-1990-1999"));
var details = new List<AdDetails?>();

foreach (var links in adListLinksScraperService.GetLinksFromPages())
{
    foreach (var link in links)
    {
        var newAdDetails = new AdDetailsScraperService(link).Call();
        if (newAdDetails != null)
        {
            details.Add(newAdDetails);
        }
    }
}

Console.WriteLine(details.Count);

