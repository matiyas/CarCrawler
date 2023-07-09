using CarCrawler.Services.Builders;
using CarCrawler.Services.Calculators.Providers.Interfaces;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace CarCrawler.Services.Calculators.Providers;

public class GoogleDistanceMatrixProvider : IDistanceMatrixProvider
{
    private readonly IAppLogger? _logger;

    public Point Origin { get; set; } = new Point(0, 0);
    public Point Destination { get; set; } = new Point(0, 0);

    public GoogleDistanceMatrixProvider()
    {
    }

    public GoogleDistanceMatrixProvider(IAppLogger logger) : this()
    {
        _logger = logger;
    }

    public DistanceMatrix? GetDistanceMatrix()
    {
        var json = GetDistanceMatrixResponsJson();

        if (json == null)
        {
            return null;
        }

        var distanceMeters = json.SelectToken("rows[0].elements[0].distance.value")?.Value<int>();
        var durationMinutes = json.SelectToken("rows[0].elements[0].duration.value")?.Value<int>();

        TimeSpan? duration = durationMinutes == null ? null : new TimeSpan(0, 0, durationMinutes.Value);
        var distanceMatrix = new DistanceMatrix
        {
            Origin = Origin,
            Destination = Destination,
            Duration = duration,
            DistanceMeters = distanceMeters
        };

        return distanceMatrix;
    }

    private JObject? GetDistanceMatrixResponsJson()
    {
        var responseBody = GetDistanceMatrixResponseBody();
        if (responseBody == null)
        {
            return null;
        }

        return JObject.Parse(responseBody);
    }

    private string? GetDistanceMatrixResponseBody()
    {
        var responseMessage = GetDistanceMatrixResponseMessage();

        if (responseMessage == null || !responseMessage.IsSuccessStatusCode)
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

        using (var client = new HttpClient())
        {
            try
            {
                var requestUri = BuildUri();
                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("Accept", "application/json");
                request.Headers.Add("User-Agent", "Mozilla/5.0");

                response = client.SendAsync(request).Result;
            }
            catch (HttpRequestException ex)
            {
                HandleRequestError(ex.Message);
            }
        }

        return response;
    }

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