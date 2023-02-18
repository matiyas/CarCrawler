using CarCrawler.Services.Scrapers;

var adListLinksScraperService = new AdListLinksScraperService(@"https://www.otomoto.pl/osobowe/honda/civic");
var links = adListLinksScraperService.Call();

Console.WriteLine(string.Join("\n", links));

