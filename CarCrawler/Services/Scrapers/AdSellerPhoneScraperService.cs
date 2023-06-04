﻿using CarCrawler;
using CarCrawler.Database;
using System.Text.Json;

internal class AdSellerPhoneScraperService
{
    private readonly AdDetails _adDetails;

    public AdSellerPhoneScraperService(AdDetails adDetails) => _adDetails = adDetails;

    public void GetSellerPhonesFromOfferId(string? offerId)
    {
        if (offerId == null) return;

        using var client = new HttpClient();
        try
        {
            var requestUri = @$"https://www.otomoto.pl/ajax/misc/contact/all_phones/{offerId}/"!;
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("User-Agent", "Mozilla/5.0");

            var response = client.SendAsync(request).Result;
            if (!response.IsSuccessStatusCode)
            {
                HandleRequestError(response.ReasonPhrase);
                return;
            }

            var responseBody = response.Content.ReadAsStringAsync().Result;
            var jsonDocument = JsonDocument.Parse(responseBody);
            var data = jsonDocument.RootElement;

            _adDetails.SellerPhones = data.EnumerateArray().Select(e => e.GetProperty("number").ToString());
        }
        catch (HttpRequestException ex)
        {
            HandleRequestError(ex.Message);
        }
    }

    private static void HandleRequestError(string? message)
    {
        Logger.Log("An unexpected error occurred:");
        Logger.Log(message ?? "unknown error");
    }
}