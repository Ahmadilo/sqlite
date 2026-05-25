using Microsoft.Data.Sqlite;

namespace Sqlite.Utils
{
    public static class TablePrinter
    {
        public static void Print(SqliteDataReader reader)
        {
            var columns = reader.FieldCount;
            var colWidths = new int[columns];
            var rows = new List<string[]>();

            for (int i = 0; i < columns; i++)
            {
                colWidths[i] = reader.GetName(i).Length;
            }

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

            for (int i = 0; i < columns; i++)
            {
                var name = reader.GetName(i);

                Console.Write(name.PadRight(colWidths[i] + 2));
            }

            Console.WriteLine();

            for (int i = 0; i < columns; i++)
            {
                Console.Write(new string('-', colWidths[i]) + "  ");
            }

            Console.WriteLine();

            foreach (var row in rows)
            {
                for (int i = 0; i < columns; i++)
                {
                    Console.Write(row[i].PadRight(colWidths[i] + 2));
                }

                Console.WriteLine();
            }
        }
    }
}