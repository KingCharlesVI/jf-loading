using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.SplashScreen.Configuration;

public class PluginConfiguration : BasePluginConfiguration
{
    public PluginConfiguration()
    {
        Enabled = true;
        LogoMode = "Url";
        LogoUrl = string.Empty;
        UploadedLogoFileName = string.Empty;
        BackgroundColor = "#000000";
        TextColor = "#ffffff";
        ProgressBarColor = "#ffffff";
        LoadingMessage = string.Empty;
        MinimumDisplayDurationMs = 1500;
    }

    public bool Enabled { get; set; }

    /// <summary>"Url" or "Upload".</summary>
    public string LogoMode { get; set; }

    public string LogoUrl { get; set; }

    public string UploadedLogoFileName { get; set; }

    public string BackgroundColor { get; set; }

    public string TextColor { get; set; }

    public string ProgressBarColor { get; set; }

    public string LoadingMessage { get; set; }

    public int MinimumDisplayDurationMs { get; set; }
}
