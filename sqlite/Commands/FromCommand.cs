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

            fromCommand.Add(fromArgument);
            fromCommand.Add(pathOption);

            fromCommand.SetAction((context) =>
            {
                var table = context.GetValue(fromArgument);
                var path = context.GetValue(pathOption);

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
                command.CommandText = $"SELECT * FROM {table}";

                using var reader = command.ExecuteReader();

                TablePrinter.Print(reader);
            });

            return fromCommand;
        }
    }
}