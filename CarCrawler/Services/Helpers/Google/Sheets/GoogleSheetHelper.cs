using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace CarCrawler.Services.Helpers.Google.Sheets;

internal class GoogleSheetHelper
{
    public SheetsService Service { get; set; } = default!;
    const string ApplicationName = "CarCrawler";
    static public string[] Scopes = { SheetsService.Scope.Spreadsheets };

    public GoogleSheetHelper ()
    {
        InitializeService();
    }

    private void InitializeService ()
    {
        var credential = GetCredentialsFromFile();
        var initializer = new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName
        };

        Service = new SheetsService(initializer);
    }

    private static GoogleCredential GetCredentialsFromFile ()
    {
        GoogleCredential credential;
        using (var stream = new FileStream("config/google/sheets/client_secrets.json", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
        }

        return credential;
    }
}