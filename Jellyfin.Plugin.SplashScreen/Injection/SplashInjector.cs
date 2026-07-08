using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.SplashScreen.Injection;

public static class SplashInjector
{
    private const string Marker = "<!-- jf-loading:splash -->";

    private static readonly object Lock = new();

    public static void Apply(string webPath, ILogger logger)
    {
        lock (Lock)
        {
            var indexPath = Path.Combine(webPath, "index.html");
            if (!File.Exists(indexPath))
            {
                logger.LogWarning("index.html not found at {Path}, skipping splash screen injection", indexPath);
                return;
            }

            var html = File.ReadAllText(indexPath);
            if (html.Contains(Marker))
            {
                return;
            }

            var backupPath = indexPath + ".jf-loading-original";
            if (!File.Exists(backupPath))
            {
                File.Copy(indexPath, backupPath);
            }

            var snippet = Marker + "\n<script defer src=\"/web/SplashScreen/loader.js\"></script>\n";
            var patched = Regex.Replace(html, "(</head>)", snippet + "$1", RegexOptions.IgnoreCase, System.TimeSpan.FromSeconds(1));
            File.WriteAllText(indexPath, patched);
            logger.LogInformation("Injected splash screen loader into {Path}", indexPath);
        }
    }

    public static void Revert(string webPath, ILogger logger)
    {
        lock (Lock)
        {
            var indexPath = Path.Combine(webPath, "index.html");
            var backupPath = indexPath + ".jf-loading-original";
            if (File.Exists(backupPath))
            {
                File.Copy(backupPath, indexPath, overwrite: true);
                logger.LogInformation("Restored original index.html at {Path}", indexPath);
            }
        }
    }
}
