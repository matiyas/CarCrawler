using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace CarCrawler.Services.Helpers.Google.Sheets;

public class GoogleSheetHelper
{
    private static readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
    private const string ApplicationName = "CarCrawler";

    public SheetsService Service { get; set; } = default!;

    public GoogleSheetHelper()
    {
        InitializeService();
    }

    private void InitializeService()
    {
        var credential = GetCredentialsFromFile();
        var initializer = new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        };

        Service = new SheetsService(initializer);
    }

    private static GoogleCredential GetCredentialsFromFile()
    {
        using var stream = new FileStream("Configuration/google/sheets/client_secrets.json", FileMode.Open, FileAccess.Read);

        return GoogleCredential.FromStream(stream).CreateScoped(_scopes);
    }
}