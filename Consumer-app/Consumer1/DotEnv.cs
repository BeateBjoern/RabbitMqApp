using System;
using System.IO;

namespace Util
{
    // A utility class for loading environment variables from a .env file
    public static class DotEnv
    {
        // Load environment variables from the specified file path
        public static void Load(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
                return;

            // Read each line from the file
            foreach (var line in File.ReadAllLines(filePath))
            {
                // Split the line into key-value pairs
                var parts = line.Split(
                    '=',
                    StringSplitOptions.RemoveEmptyEntries);

                // Skip lines that do not have exactly two parts
                if (parts.Length != 2)
                    continue;

                // Set the environment variable using the key-value pair
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}