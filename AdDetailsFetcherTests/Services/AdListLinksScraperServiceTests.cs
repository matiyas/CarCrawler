using AdDetailsFetcher.Services;
using AppLogger;
using HtmlAgilityPack;
using Moq;
using Moq.Protected;

namespace AdDetailsFetcherTests.Services;

public class AdListLinksScraperServiceTests
{
    [Fact]
    public void GetLinksFromSinglePage_Success_ReturnsUrls()
    {
        // Arrange
        var uri = new Uri("https://www.otomoto.pl/osobowe/honda/civic/ver-1-8-sport");
        var loggerMock = new Mock<IAppLogger>();
        var htmlDocumentPage1 = new HtmlDocument();
        htmlDocumentPage1.Load("TestData/otomoto/offers-page-1.html");
        var htmlDocumentPage2 = new HtmlDocument();
        htmlDocumentPage2.Load("TestData/otomoto/offers-page-2.html");
        var scraperMock = new Mock<AdListLinksScraperService>(uri, loggerMock.Object);
        scraperMock
            .Protected()
            .Setup<HtmlNode>("GetHtmlDocNodeForCurrentPage")
            .Returns(() =>
            {
                if (scraperMock.Object.AdListLinkWithPage.ToString() == $"{uri}?page=2")
                    return htmlDocumentPage2.DocumentNode;

                return htmlDocumentPage1.DocumentNode;
            });
        var expectedLinksPages = new[]
        {
            new[]
            {
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-stan-perfekcyjny-1-wlasciciel-bezwypadkowa-ID6FAZ69.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-ix-salon-pl-serwis-aso-bezwypadkowa-1-8-142-km-sport-ID6FKrob.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-140km-ID6FL2ei.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-ID6FMj4A.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-pierwszy-wlasciciel-stan-bardzo-dobry-serwisowany-garazowany-ID6FFZOe.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-142km-krajowy-wersja-sport-ID6FJiOe.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-sport-ID6FJdqD.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-sport-1-8i-ID6FMew5.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-polski-salon-bezwypadkowa-osoba-prywatna-ID6FMay4.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-ufo-stan-bardzo-dobry-ID6FMafs.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-142-km-biala-perla-swiezo-sprowadzona-bezwypadkowa-serwisowana-ID6FIoMA.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8benz-140km-6-bieg-super-stan-2kpl-kol-ID6FvOOp.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-pierwszy-wlasciciel-zadbana-honda-ID6FJ5Mk.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8ivtecsportlift-oplacona-nowa-kamera-led-bezwypdk-aso-niemcy-ID6FLhuK.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-v-tec-140-km-sport-113-tys-super-stan-skora-ID6FJ0LN.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-vtec-manual-6-biegow-wersja-sport-ID6FIaw3.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-2010r-1-8-140km-9rzeb-149tys-km-klimatronic-kamera-ID6FIQIl.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-i-vtec-140km-bardzo-dobry-stan-prywatny-wlasciciel-ID6FKvDH.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-benzyna-2009-lift-zarejestrowany-ID6FHgm7.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-2012-r-1-8-benzyna-z-lpg-ID6Fd6Kk.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-viii-1-8-sport-ID6FLXvz.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-tourer-sport-serwis-aso-ID6FLX8d.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-sport-ID6FLX2p.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-i-vtec-ii-wl-rzeczprzebieg-kamera-cofania-hak-2xkola-bezwypadkowy-ID6FhfUu.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-i-vtec-sport-2012r-i-rej-i-2013r-rzeczprzebieg-climatronik-bezwy-ID6FjT8W.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-wersja-sport-ID6FGxHP.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-viii-1-8-sport-ID6FK2Px.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-samochod-z-polskiego-salonu-niski-przebieg-ID6FBfqN.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-ufo-viii-1-8-benzyna-niski-przebieg-doskonaly-stan-ID6F7ZT5.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-2009-sliczny-calutki-w-oryginale-ID6FL276.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8i-sport-bezwypadkowa-ze-szwajcarii-ID6FHMnk.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-benzyna-gaz-orginalny-lakier-po-lifcie-ID6FIuCH.html")
            },
            new[]
            {
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-v-tec-ID6FLGnv.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-sport-oplacony-gwarancja-3-m-ce-ID6FyVDh.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-ID6FGLSE.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-sport-lift-ID6FjIWx.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-rok-prod-2007-i-rej-styczen-2008-1-8-benzyna-140km-ID6FLtbn.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-sport-5d-bezwypadkowy-salon-pl-aso-ID6FLo4i.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-drugi-wlasciciel-serwisowany-stan-idealny-ID6FzWSP.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-ID6FJXKV.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-benzyna-serwisowany-bezwypadkowy-ID6FDpFm.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-140-km-klimatronik-siedzenia-podgrzewane-ID6FsENw.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-viii-1-8-2010-ID6FLgZe.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-ufo-2008-prywatnie-ID6FLfUt.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-1-8-2007r-ID6FIdr2.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-viii-1-8-i-vtec-140-km-ID6FL4Qv.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-honda-civic-viii-2010-bezwypadkowy-i-wlasciciel-polski-salon-ID6FJJto.html"),
                new Uri("https://www.otomoto.pl/osobowe/oferta/honda-civic-1-8-140km-sport-ID6FKf8B.html")
            }
        };

        // Act
        var result = scraperMock.Object.GetLinksFromPages();
        var resultArray = result.ToArray();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<IEnumerable<Uri>>>(result);
        Assert.Equal(2, resultArray.Length);
        Assert.All(expectedLinksPages[0], link => Assert.Contains(link, resultArray[0]));
        Assert.All(expectedLinksPages[1], link => Assert.Contains(link, resultArray[1]));
        Assert.Equal(expectedLinksPages[0].Length, resultArray[0].Count());
        Assert.Equal(expectedLinksPages[1].Length, resultArray[1].Count());
    }
}