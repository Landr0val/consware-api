using System;
using System.IO;

namespace consware_api.Infrastructure.Configuration
{
    public static class EnvironmentLoader
    {
        public static void LoadFromFile(string filePath = ".env")
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Environment file {filePath} not found. Using default values.");
                return;
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2)
                    continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (string.IsNullOrEmpty(key))
                    continue;

                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }
}
