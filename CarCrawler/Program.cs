global using CarCrawler.Models;
using CarCrawler.Services.Generators.CSV;

//var adListLinksScraperService = new AdListLinksScraperService(new Uri(@"https://www.otomoto.pl/osobowe/honda/civic"));

var uri = new Uri($@"https://www.otomoto.pl/osobowe/bmw/seria-3?search%5Bfilter_enum_generation%5D=gen-e36-1990-1999");
var generator = new AdsListReportCsvGeneratorService(uri);

generator.Call();
