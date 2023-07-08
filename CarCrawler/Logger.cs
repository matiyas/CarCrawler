﻿using CarCrawler.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace CarCrawler.Loggers;

public class Logger : ILogger
{
    private readonly ILogger<Logger> _logger;

    public Logger()
    {
        var logLevel = Configuration.Get.GetValue<LogLevel>("Logging:LogLevel:Default");
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole(options => options.FormatterName = nameof(CustomConsoleFormatter))
                .AddConsoleFormatter<CustomConsoleFormatter, SimpleConsoleFormatterOptions>();
            builder.AddFile("car_crawler.log");
            builder.SetMinimumLevel(logLevel);
        });

        _logger = loggerFactory.CreateLogger<Logger>();
    }

    public void Log(string message) => _logger.LogInformation(message);
}