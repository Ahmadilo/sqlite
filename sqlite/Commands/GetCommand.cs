using System.CommandLine;
using Sqlite.Services;

namespace Sqlite.Commands
{
    public static class GetCommand
    {
        public static Command Create()
        {
            var pathOption = new Option<string>("--path")
            {
                Description = "The Path of Sqlite Database.",
                Arity = ArgumentArity.ExactlyOne,
            };

            var tableNamesOption = new Option<bool>("--table-names")
            {
                Description = "Print all table names."
            };

            var getCommand = new Command("get", "Get information from SQLite");

            getCommand.Add(pathOption);
            getCommand.Add(tableNamesOption);

            getCommand.SetAction((context) =>
            {
                var path = context.GetValue(pathOption);
                var tableNames = context.GetValue(tableNamesOption);

                path = ConfigService.ResolvePath(path);

                if (string.IsNullOrWhiteSpace(path))
                {
                    Console.WriteLine("Database path is required.");
                    return;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine("Database file not found.");
                    return;
                }

                using var connection = SqliteService.Open(path);

                if (tableNames)
                {
                    var command = connection.CreateCommand();

                    command.CommandText =
                    @"
                        SELECT name
                        FROM sqlite_master
                        WHERE type='table'
                        ORDER BY name;
                    ";

                    using var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Console.WriteLine(reader.GetString(0));
                    }

                    return;
                }

                Console.WriteLine("No get option provided.");
            });

            return getCommand;
        }
    }
}