using System;
using System.IO;
using System.Text.Json;
using IndustrialIoTManager.Model;

namespace IndustrialIoTManager.Helpers;

public static class AppConfiguration
{
    private const string ConfigurationFileName = "appsettings.json";

    public static DataAccessOptions LoadDataAccessOptions()
    {
        try
        {
            var configPath = ResolveConfigurationPath();
            if (configPath is null)
            {
                return new DataAccessOptions();
            }

            using var stream = File.OpenRead(configPath);
            using var document = JsonDocument.Parse(stream);

            if (!document.RootElement.TryGetProperty("DataAccess", out var dataAccessSection))
            {
                return new DataAccessOptions();
            }

            var modeText = dataAccessSection.TryGetProperty("Mode", out var modeElement)
                ? modeElement.GetString()
                : null;

            var connectionString = dataAccessSection.TryGetProperty("SqlServerConnectionString", out var connectionElement)
                ? connectionElement.GetString()
                : DataAccessOptions.DefaultSqlServerConnectionString;

            var mode = Enum.TryParse(modeText, ignoreCase: true, out DataAccessMode parsedMode)
                ? parsedMode
                : DataAccessMode.InMemory;

            return new DataAccessOptions
            {
                Mode = mode,
                SqlServerConnectionString = string.IsNullOrWhiteSpace(connectionString)
                    ? DataAccessOptions.DefaultSqlServerConnectionString
                    : connectionString
            };
        }
        catch
        {
            return new DataAccessOptions();
        }
    }

    private static string? ResolveConfigurationPath()
    {
        var basePath = Path.Combine(AppContext.BaseDirectory, ConfigurationFileName);
        if (File.Exists(basePath))
        {
            return basePath;
        }

        var currentPath = Path.Combine(Directory.GetCurrentDirectory(), ConfigurationFileName);
        if (File.Exists(currentPath))
        {
            return currentPath;
        }

        return null;
    }
}
