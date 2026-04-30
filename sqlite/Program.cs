using System.CommandLine;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Sqlite
{
    class Program
    {
        public static string? FindConfigFile(string startDir)
        {
            var dir = new DirectoryInfo(startDir);

            while (dir != null)
            {
                var configPath = Path.Combine(dir.FullName, "sqlite.config");

                if (File.Exists(configPath))
                    return configPath;

                dir = dir.Parent;
            }

            return null;
        }

        public static string? ReadPathFromConfig(string configFile)
        {
            foreach (var line in File.ReadAllLines(configFile))
            {
                var trimmed = line.Trim();

                if (trimmed.StartsWith("path=", StringComparison.OrdinalIgnoreCase))
                {
                    return trimmed.Substring(5).Trim();
                }
            }

            return null;
        }

        public static int Main (string[] args) 
        {
            var root = new RootCommand("to Expling The SQLite");

            var pathOption = new Option<string>("--path") 
            {
                Description = "The Path of Sqlite Database.",
                Arity = ArgumentArity.ExactlyOne,
            };

            root.Options.Add(pathOption);

            var readCommand = new Command("from", "read Table from SQLite file");

            var fromOption = new Argument<string>(name: "table") { Description = "Select the Table By Name" };

            readCommand.Add(fromOption);
            readCommand.Add(pathOption);

            readCommand.SetAction((context) => 
            {
                var from = context.GetValue(fromOption);
                var path = context.GetValue(pathOption);

                if (string.IsNullOrWhiteSpace(path))
                {
                    try
                    {
                        var currentDir = Directory.GetCurrentDirectory();
                        var configFile = FindConfigFile(currentDir);

                        if (configFile != null)
                        {
                            var relativePath = ReadPathFromConfig(configFile);

                            if (!string.IsNullOrWhiteSpace(relativePath))
                            {
                                // مهم: المسار يكون نسبةً إلى مكان config
                                var fullPath = Path.GetFullPath(
                                    Path.Combine(Path.GetDirectoryName(configFile)!, relativePath)
                                );

                                if (File.Exists(fullPath))
                                {
                                    path = fullPath;
                                }
                            }
                        }
                    }
                    catch
                    {
                        path = null; // تجاهل الأخطاء
                    }
                }

                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(path))
                {
                    Console.WriteLine("path and table name are required.");
                    return;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine("Database file not found.");
                    return;
                }

                var connectionString = $"Data Source={path}";

                using var connection = new SqliteConnection(connectionString);
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM {from}";

                using var reader = command.ExecuteReader();

                var columns = reader.FieldCount;
                var colWidths = new int[columns];
                var rows = new List<string[]>();

                // تحديد عرض الأعمدة بناءً على أسماء الأعمدة
                for (int i = 0; i < columns; i++)
                {
                    colWidths[i] = reader.GetName(i).Length;
                }

                // قراءة البيانات وتحديث العرض
                while (reader.Read())
                {
                    var row = new string[columns];

                    for (int i = 0; i < columns; i++)
                    {
                        var value = reader.GetValue(i)?.ToString() ?? "NULL";
                        row[i] = value;

                        if (value.Length > colWidths[i])
                            colWidths[i] = value.Length;
                    }

                    rows.Add(row);
                }

                // طباعة أسماء الأعمدة
                for (int i = 0; i < columns; i++)
                {
                    var name = reader.GetName(i);
                    Console.Write(name.PadRight(colWidths[i] + 2));
                }
                Console.WriteLine();

                // خط فاصل
                for (int i = 0; i < columns; i++)
                {
                    Console.Write(new string('-', colWidths[i]) + "  ");
                }
                Console.WriteLine();

                // طباعة البيانات
                foreach (var row in rows)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        Console.Write(row[i].PadRight(colWidths[i] + 2));
                    }
                    Console.WriteLine();
                }
            });

            root.Add(readCommand);

            return root.Parse(args).Invoke();
        }
    }
}
