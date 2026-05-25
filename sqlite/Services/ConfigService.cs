namespace Sqlite.Services
{
    public static class ConfigService
    {
        public static string? ResolvePath(string? path)
        {
            if (!string.IsNullOrWhiteSpace(path))
                return path;

            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var configFile = FindConfigFile(currentDir);

                if (configFile != null)
                {
                    var relativePath = ReadPathFromConfig(configFile);

                    if (!string.IsNullOrWhiteSpace(relativePath))
                    {
                        var fullPath = Path.GetFullPath(
                            Path.Combine(Path.GetDirectoryName(configFile)!, relativePath)
                        );

                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

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
    }
}