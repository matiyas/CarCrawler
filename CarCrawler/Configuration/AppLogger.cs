using CarCrawler.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace CarCrawler.Configuration;

public class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;
    private readonly IAppConfiguration _configuration;

    public AppLogger(IAppConfiguration configuration)
    {
        _configuration = configuration;

        var logLevel = _configuration.GetValue<LogLevel>("Logging:LogLevel:Default");
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter))
                .AddConsoleFormatter<CustomConsoleFormatter, SimpleConsoleFormatterOptions>();
            builder.AddFile("car_crawler.log");
            builder.SetMinimumLevel(logLevel);
        });

        _logger = loggerFactory.CreateLogger<AppLogger>();
    }

    public void Log(string message) => _logger.LogInformation(message);
}