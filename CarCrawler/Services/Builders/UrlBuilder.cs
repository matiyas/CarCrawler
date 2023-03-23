using System.Text;

namespace CarCrawler.Services.Builders;

public class UrlBuilder
{
    public static string BuildUrl(string baseUrl, Dictionary<string, string> parameters)
    {
        StringBuilder builder = new StringBuilder(baseUrl);

        if (parameters.Count == 0)
        {
            return builder.ToString();
        }
            
        builder.Append('?');
        foreach (KeyValuePair<string, string> pair in parameters)
        {
            builder.Append(pair.Key);
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(pair.Value));
            builder.Append('&');
        }
        builder.Length--; // remove last "&" character

        return builder.ToString();
    }
}
