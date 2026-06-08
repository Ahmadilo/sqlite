namespace Sqlite.Utils;

public static class SqlFormatter
{
    public static string FormatCreateTable(string sql)
    {
        return sql
            .Replace("(", "(\n    ")
            .Replace(", ", ",\n    ")
            .Replace(")", "\n)");
    }
}