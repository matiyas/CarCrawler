using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace CarCrawler.Formatters;

public sealed class CustomConsoleFormatter : ConsoleFormatter
{
    public CustomConsoleFormatter() : base(nameof(CustomConsoleFormatter))
    { }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var timestamp = DateTimeOffset.Now.ToString("[yyyy-MM-ddTHH:mm:ss.fffffffzzz] ");
        var logLevel = logEntry.LogLevel.ToString();
        var color = GetLogLevelColor(logEntry.LogLevel);
        var logLevelString = GetLogLevelString(logEntry.LogLevel);

        textWriter.Write(timestamp);
        textWriter.Write("[");
        textWriter.WriteWithColor(logLevelString, null, color);
        textWriter.Write("]: ");
        textWriter.WriteLine(logEntry.Formatter(logEntry.State, logEntry.Exception));
    }

    private static ConsoleColor GetLogLevelColor(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => ConsoleColor.DarkGray,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Information => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,

            _ => ConsoleColor.Gray,
        };

    private static string GetLogLevelString(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => "trace",
            LogLevel.Debug => "debug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warning",
            LogLevel.Error => "error",
            LogLevel.Critical => "critical",

            _ => "default",
        };
}