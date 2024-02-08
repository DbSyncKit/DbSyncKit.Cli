using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSyncKit.Cli.Extensions
{
    /// <summary>
    /// Provides extension methods for logging messages with ANSI console styling.
    /// </summary>
    public class LoggerExtensions
    {
        /// <summary>
        /// Logs a message with the "LOG" tag and ANSI styling.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public static void Log(string message)
        {
            AnsiConsole.MarkupLine($"[bold gray]LOG:[/] {message}");
        }

        /// <summary> 
        /// Logs a success message with the "SUCCESS" tag and ANSI styling.
        /// </summary>
        /// <param name="message">The success message to log.</param>
        public static void Success(string message)
        {
            AnsiConsole.MarkupLine($"[bold green]SUCCESS:[/] {message}");
        }

        /// <summary>
        /// Logs a warning message with the "WARN" tag and ANSI styling.
        /// </summary>
        /// <param name="message">The warning message to log.</param>
        public static void Warn(string message)
        {
            AnsiConsole.MarkupLine($"[bold yellow]WARN:[/] {message}");
        }

        /// <summary>
        /// Logs an error message with the "ERROR" tag and ANSI styling.
        /// </summary>
        /// <param name="message">The error message to log.</param>
        public static void Error(string message)
        {
            AnsiConsole.MarkupLine($"[bold red]ERROR:[/] {message}");
        }

        /// <summary>
        /// Logs a debug message with the "DEBUG" tag and ANSI styling.
        /// </summary>
        /// <param name="message">The debug message to log.</param>
        public static void Debug(string message)
        {
            AnsiConsole.MarkupLine($"[bold blue]DEBUG:[/] {message}");
        }
    }
}
