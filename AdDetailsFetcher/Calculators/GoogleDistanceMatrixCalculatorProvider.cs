using AdDetailsFetcher.Utils;
using AdDetailsFetcher.Models;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using AppLogger;

namespace AdDetailsFetcher.Calculators;

public class GoogleDistanceMatrixCalculatorProvider : IDistanceMatrixCalculatorProvider
{
    private readonly IAppLogger? _logger;

    public Point Origin { get; set; } = new Point(0, 0);
    public Point Destination { get; set; } = new Point(0, 0);

    public GoogleDistanceMatrixCalculatorProvider()
    {
    }

    public GoogleDistanceMatrixCalculatorProvider(IAppLogger logger) : this()
    {
        _logger = logger;
    }

    public DistanceMatrix? GetDistanceMatrix()
    {
        var json = GetDistanceMatrixResponseJson();
        if (json is null) return null;

        var distanceMeters = json.SelectToken("rows[0].elements[0].distance.value")?.Value<int>();
        var durationMinutes = json.SelectToken("rows[0].elements[0].duration.value")?.Value<int>();

        TimeSpan? duration = durationMinutes is null ? null : new TimeSpan(0, 0, durationMinutes.Value);
        var distanceMatrix = new DistanceMatrix
        {
            Origin = Origin,
            Destination = Destination,
            Duration = duration,
            DistanceMeters = distanceMeters
        };

        return distanceMatrix;
    }

    private JObject? GetDistanceMatrixResponseJson()
    {
        var responseBody = GetDistanceMatrixResponseBody();
        if (responseBody is null) return null;

        return JObject.Parse(responseBody);
    }

    private string? GetDistanceMatrixResponseBody()
    {
        var responseMessage = GetDistanceMatrixResponseMessage();
        if (responseMessage is null) return null;

        if (!responseMessage.IsSuccessStatusCode)
        {
            HandleRequestError(responseMessage?.ReasonPhrase);
            return null;
        }

        var responseBody = responseMessage.Content.ReadAsStringAsync().Result;

        return responseBody;
    }

    private HttpResponseMessage? GetDistanceMatrixResponseMessage()
    {
        HttpResponseMessage? response = default;

        using (var client = GetHttpClient)
        {
            try
            {
                response = SendRequest(client);
            }
            catch (HttpRequestException ex)
            {
                HandleRequestError(ex.Message);
            }
        }

        return response;
    }

    private HttpResponseMessage SendRequest(HttpClient client)
    {
        var requestUri = BuildUri();
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "Mozilla/5.0");

        return client.SendAsync(request).Result;
    }

    protected virtual HttpClient GetHttpClient => new();

    private void HandleRequestError(string? message)
    {
        _logger?.Log("An unexpected error occurred:");
        _logger?.Log(message ?? "unknown error");
    }

    private Uri BuildUri()
    {
        var apiKey = GetGoogleMapsApiKey();
        var baseUrl = "https://maps.googleapis.com/maps/api/distancematrix/json";
        var parameters = new Dictionary<string, string>
        {
            { "origins", $"{Origin.X},{Origin.Y}" },
            { "destinations", $"{Destination.X},{Destination.Y}"  },
            { "key", apiKey },
        };
        var stringUrl = UrlBuilder.BuildUrl(baseUrl, parameters);

        return new Uri(stringUrl);
    }

    private static string GetGoogleMapsApiKey()
    {
        var jsonString = File.ReadAllText("Configuration/google/maps/client_secrets.json");
        var jsonDocument = JsonDocument.Parse(jsonString);
        var data = jsonDocument.RootElement;
        var key = data.EnumerateObject().First(entry => entry.Name == "private_key").Value.ToString();

        return key;
    }
}