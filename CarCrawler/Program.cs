using CarCrawler.Services.Scrapers;

var adListLinksScraperService = new AdListLinksScraperService(new Uri(@"https://www.otomoto.pl/osobowe/honda/civic"));
var links = adListLinksScraperService.Call();
var details = links.Select(link => new AdDetailsScraperService(link).Call()).ToList();

Console.WriteLine(string.Join("\n", links));

