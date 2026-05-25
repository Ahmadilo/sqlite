using System.CommandLine;
using Sqlite.Commands;

namespace Sqlite
{
    class Program
    {
        public static int Main(string[] args)
        {
            var root = new RootCommand("to Expling The SQLite");

            root.Add(FromCommand.Create());

            return root.Parse(args).Invoke();
        }
    }
}