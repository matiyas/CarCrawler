using AdDetailsFetcher.Models;
using AppLogger;
using System.Text.Json;

namespace AdDetailsFetcher.Services;

public class AdSellerPhoneScraperService
{
    private readonly AdDetails _adDetails;
    private readonly IAppLogger? _logger;
    private readonly HttpClient _httpClient;

    public AdSellerPhoneScraperService(AdDetails adDetails)
    {
        _adDetails = adDetails;
        _httpClient = new HttpClient();
    }

    public AdSellerPhoneScraperService(AdDetails adDetails, IAppLogger logger) : this(adDetails)
    {
        _logger = logger;
    }

    public AdSellerPhoneScraperService(AdDetails adDetails, IAppLogger logger, HttpClient httpClient) : this(adDetails, logger)
    {
        _httpClient = httpClient;
    }

    public AdSellerPhoneScraperService(AdDetails adDetails, HttpClient httpClient) : this(adDetails)
    {
        _httpClient = httpClient;
    }

    public async Task GetSellerPhonesFromOfferId(string? offerId)
    {
        if (offerId is null) return;

        using var client = _httpClient;
        try
        {
            var requestUri = @$"https://www.otomoto.pl/ajax/misc/contact/all_phones/{offerId}/"!;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Mozilla/5.0");

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                HandleRequestError(response.ReasonPhrase);
                return;
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(responseBody);
            var data = jsonDocument.RootElement;

            _adDetails.SellerPhones = data.EnumerateArray().Select(e => e.GetProperty("number").ToString());
        }
        catch (HttpRequestException ex)
        {
            HandleRequestError(ex.Message);
        }
    }

    private void HandleRequestError(string? message)
    {
        _logger?.Log("An unexpected error occurred:");
        _logger?.Log(message ?? "unknown error");
    }
}