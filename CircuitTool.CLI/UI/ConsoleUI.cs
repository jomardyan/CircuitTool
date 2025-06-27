using System;
using System.Threading.Tasks;
using Spectre.Console;
using CircuitTool.CLI.Commands;

namespace CircuitTool.CLI.UI
{
    /// <summary>
    /// Provides console UI utilities and interactive menus
    /// </summary>
    internal static class ConsoleUI
    {
        /// <summary>
        /// Displays the welcome banner
        /// </summary>
        public static void DisplayWelcomeBanner()
        {
            var rule = new Rule("[bold blue]CircuitTool Interactive CLI[/]")
            {
                Style = Style.Parse("blue")
            };
            AnsiConsole.Write(rule);
            
            var panel = new Panel(
                new Markup("[dim]A comprehensive C# library for electrical engineering calculations\n" +
                          "Test the framework capabilities interactively[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("blue"),
                Padding = new Padding(2, 1)
            };
            
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }

        /// <summary>
        /// Creates a styled table for displaying calculation results
        /// </summary>
        public static Table CreateResultsTable(string title, params string[] headers)
        {
            var table = new Table()
                .Title(title)
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Blue);

            foreach (var header in headers)
            {
                table.AddColumn(new TableColumn(header).Centered());
            }

            return table;
        }

        /// <summary>
        /// Displays an error message
        /// </summary>
        public static void DisplayError(string message, Exception? exception = null)
        {
            AnsiConsole.Write(new Panel(new Markup($"[red]Error:[/] {message}"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("red")
            });

            if (exception != null)
            {
                AnsiConsole.WriteException(exception);
            }
        }

        /// <summary>
        /// Displays a success message
        /// </summary>
        public static void DisplaySuccess(string message)
        {
            AnsiConsole.Write(new Panel(new Markup($"[green]Success:[/] {message}"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("green")
            });
        }

        /// <summary>
        /// Displays a warning message
        /// </summary>
        public static void DisplayWarning(string message)
        {
            AnsiConsole.Write(new Panel(new Markup($"[yellow]Warning:[/] {message}"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = Style.Parse("yellow")
            });
        }

        /// <summary>
        /// Prompts user for a double value with validation
        /// </summary>
        public static double PromptForDouble(string prompt, double? min = null, double? max = null)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<double>(prompt)
                    .ValidationErrorMessage("[red]Invalid number format[/]")
                    .Validate(value =>
                    {
                        if (min.HasValue && value < min.Value)
                            return ValidationResult.Error($"Value must be at least {min.Value}");
                        if (max.HasValue && value > max.Value)
                            return ValidationResult.Error($"Value must be at most {max.Value}");
                        return ValidationResult.Success();
                    }));
        }

        /// <summary>
        /// Prompts user for a string value
        /// </summary>
        public static string PromptForString(string prompt, bool allowEmpty = false)
        {
            var textPrompt = new TextPrompt<string>(prompt);
            
            if (!allowEmpty)
            {
                textPrompt.ValidationErrorMessage("[red]Value cannot be empty[/]")
                         .Validate(value => !string.IsNullOrWhiteSpace(value) 
                             ? ValidationResult.Success() 
                             : ValidationResult.Error("Value cannot be empty"));
            }

            return AnsiConsole.Prompt(textPrompt);
        }

        /// <summary>
        /// Creates a progress task for long-running operations
        /// </summary>
        public static async Task WithProgress(string description, Func<ProgressTask, Task> work)
        {
            await AnsiConsole.Progress()
                .AutoRefresh(true)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new ElapsedTimeColumn(),
                    new SpinnerColumn()
                })
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask(description);
                    await work(task);
                });
        }

        /// <summary>
        /// Creates a progress task for long-running operations that return a value
        /// </summary>
        public static async Task<T> WithProgress<T>(string description, Func<ProgressTask, Task<T>> work)
        {
            return await AnsiConsole.Progress()
                .AutoRefresh(true)
                .Columns(new ProgressColumn[]
                {
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn(),
                    new PercentageColumn(),
                    new ElapsedTimeColumn(),
                    new SpinnerColumn()
                })
                .StartAsync(async ctx =>
                {
                    var task = ctx.AddTask(description);
                    return await work(task);
                });
        }
    }
}
