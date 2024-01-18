using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DbSyncKit.Cli.Commands
{
    public class StatusCommand : Command<StatusCommand.Settings>
    {
        #region Settings

        public class Settings : CommandSettings { }

        #endregion

        #region Execution

        public override int Execute(CommandContext context, Settings settings)
        {
            AnsiConsole.MarkupLine("[yellow]Checking status...[/]");

            DisplayDotnetToolVersion();
            DisplayInstalledVersion();
            DisplayNuGetVersion();

            return 0;
        }

        #endregion

        #region Helper Methods

        private void DisplayDotnetToolVersion()
        {
            AnsiConsole.MarkupLine($"[bold]Dotnet Tool Version:[/] {GetDotnetToolVersion()}");
        }

        private string GetDotnetToolVersion()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        return output.Trim();
                    }

                    AnsiConsole.MarkupLine($"[red]Error getting dotnet tool version: {error}[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error getting dotnet tool version: {ex.Message}[/]");
            }

            return "Unknown";
        }

        private void DisplayInstalledVersion()
        {
            AnsiConsole.MarkupLine($"[bold]Installed Version:[/] {GetInstalledVersion()}");
        }

        private string GetInstalledVersion()
        {
            try
            {
                var assembly = Assembly.GetEntryAssembly();
                var attribute = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                return attribute?.InformationalVersion ?? "Unknown";
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error getting installed version: {ex.Message}[/]");
            }

            return "Unknown";
        }

        private void DisplayNuGetVersion()
        {
            AnsiConsole.MarkupLine($"[bold]NuGet Version:[/] {GetNuGetVersion()}");
        }

        private string GetNuGetVersion()
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "nuget locals all --list",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        var nugetVersionLine = output.Split('\n')
                            .FirstOrDefault(line => line.StartsWith("httpcache", StringComparison.OrdinalIgnoreCase));

                        if (!string.IsNullOrEmpty(nugetVersionLine))
                        {
                            return nugetVersionLine.Split(' ').LastOrDefault()?.Trim() ?? "Unknown";
                        }
                    }

                    AnsiConsole.MarkupLine($"[red]Error getting NuGet version: {error}[/]");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error getting NuGet version: {ex.Message}[/]");
            }

            return "Unknown";
        }


        #endregion
    }
}
