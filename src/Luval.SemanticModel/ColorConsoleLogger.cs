using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.SemanticModel
{
    public class ColorConsoleLogger : ILogger
    {
        private readonly string _categoryName;

        public ColorConsoleLogger() : this("App")
        {

        }

        public ColorConsoleLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // No need for a scope in this implementation
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Enable all log levels for simplicity
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (exception != null)
            {
                message += $"\n{exception}";
            }

            // Set the console color based on the log level
            var originalColor = Console.ForegroundColor;

            switch (logLevel)
            {
                case LogLevel.Critical:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow; // Amber
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogLevel.Trace:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                default:
                    Console.ForegroundColor = originalColor;
                    break;
            }

            Console.WriteLine($"[{logLevel}] {_categoryName}: {message}");

            // Reset to the original color
            Console.ForegroundColor = originalColor;
        }
    }

    public class ColorConsoleLogger<TCategory> : ColorConsoleLogger, ILogger<TCategory>
    {
        public ColorConsoleLogger() : base(typeof(TCategory).Name)
        {

        }
    }
}
