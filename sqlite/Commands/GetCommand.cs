using System.CommandLine;
using Sqlite.Services;
using Sqlite.Utils;

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

            var schemaOption = new Option<string>("--schema")
            {
                Description = "Print CREATE TABLE statement."
            };
            
            var columnsOption = new Option<string>("--columns")
            {
                Description = "Print table columns."
            };


            var getCommand = new Command("get", "Get information from SQLite");

            getCommand.Add(columnsOption);
            getCommand.Add(pathOption);
            getCommand.Add(tableNamesOption);
            getCommand.Add(schemaOption);

            getCommand.SetAction((context) =>
            {
                var path = context.GetValue(pathOption);
                var tableNames = context.GetValue(tableNamesOption);
                var schema = context.GetValue(schemaOption);
                var columns = context.GetValue(columnsOption);


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

                if (!string.IsNullOrWhiteSpace(schema))
                {
                    var command = connection.CreateCommand();

                    command.CommandText =@"
                        SELECT sql
                        FROM sqlite_master
                        WHERE type = 'table'
                        AND name = $table;
                    ";

                    command.Parameters.AddWithValue("$table", schema);

                    var result = command.ExecuteScalar();

                    if (result is null || result == DBNull.Value)
                    {
                        Console.WriteLine($"Table '{schema}' not found.");
                        return;
                    }

                    Console.WriteLine(SqlFormatter.FormatCreateTable(result.ToString()!));

                    return;
                }

                if (!string.IsNullOrWhiteSpace(columns))
                {
                    var command = connection.CreateCommand();

                    command.CommandText = $"PRAGMA table_info({columns})";

                    using var reader = command.ExecuteReader();

                    var columnNames = new List<string>();

                    while (reader.Read())
                    {
                        columnNames.Add(reader["name"]?.ToString() ?? "");
                    }

                    if (columnNames.Count == 0)
                    {
                        Console.WriteLine($"Table '{columns}' not found.");
                        return;
                    }

                    var width = Math.Max(
                        "Column".Length,
                        columnNames.Max(x => x.Length)
                    );

                    string border = "+" + new string('-', width + 2) + "+";

                    Console.WriteLine(border);
                    Console.WriteLine($"| {"Column".PadRight(width)} |");
                    Console.WriteLine(border);

                    foreach (var column in columnNames)
                    {
                        Console.WriteLine($"| {column.PadRight(width)} |");
                    }

                    Console.WriteLine(border);

                    return;
                }

                Console.WriteLine("No get option provided.");
            });

            return getCommand;
        }
    }
}