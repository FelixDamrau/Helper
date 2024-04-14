using Microsoft.Extensions.Configuration;

namespace Develix.Helper;

public class AppSettings
{
    /// <summary>
    /// The absolute path of the local package cache.
    /// </summary>
    public string LocalPackageCache { get; set; } = "Net set";

    /// <summary>
    /// The absolute path that identifies the setup publish directory.
    /// </summary>
    public string PublishSetupRoot { get; init; } = "Not set";

    /// <summary>
    /// The sub path that identifies the setup build directory.
    /// </summary>
    public string SetupDirectoryIdentifier { get; init; } = "Not set";

    /// <summary>
    /// The path to the working directory. Usually only used for debugging purposes.
    /// </summary>
    public string WorkingDirectory { get; init; } = "Not set";

    private AppSettings()
    {
    }

    public static AppSettings Create()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var settings = new AppSettings();
        var settingsSection = configuration.GetSection("Settings");
        settingsSection.Bind(settings);

        return settings;
    }
}
