using System.CommandLine;
using Microsoft.Data.Sqlite;
using Sqlite.Services;
using Sqlite.Utils;

namespace Sqlite.Commands
{
    public static class FromCommand
    {
        public static Command Create()
        {
            var pathOption = new Option<string>("--path")
            {
                Description = "The Path of Sqlite Database.",
                Arity = ArgumentArity.ExactlyOne,
            };

            var fromCommand = new Command("from", "read Table from SQLite file");

            var fromArgument = new Argument<string>("table")
            {
                Description = "Select the Table By Name"
            };

            var selectOption = new Option<string[]>("--select")
            {
                Description = "Select specific columns.",
                AllowMultipleArgumentsPerToken = true,
            };

            fromCommand.Add(selectOption);
            fromCommand.Add(fromArgument);
            fromCommand.Add(pathOption);

            fromCommand.SetAction((context) =>
            {
                var table = context.GetValue(fromArgument);
                var path = context.GetValue(pathOption);
                var selectedColumns = context.GetValue(selectOption);

                path = ConfigService.ResolvePath(path);

                if (string.IsNullOrWhiteSpace(table) || string.IsNullOrWhiteSpace(path))
                {
                    Console.WriteLine("path and table name are required.");
                    return;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine("Database file not found.");
                    return;
                }

                using var connection = SqliteService.Open(path);

                var command = connection.CreateCommand();
                string columns = "*";

                if (selectedColumns is not null && selectedColumns.Length > 0)
                {
                    columns = string.Join(", ", selectedColumns);
                }

                command.CommandText = $"SELECT {columns} FROM {table}";

                using var reader = command.ExecuteReader();

                TablePrinter.Print(reader);
            });

            return fromCommand;
        }
    }
}