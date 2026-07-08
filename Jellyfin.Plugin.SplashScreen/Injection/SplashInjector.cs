using System;
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

            var backupPath = indexPath + ".jf-loading-original";
            if (!File.Exists(backupPath))
            {
                var current = File.ReadAllText(indexPath);
                if (current.Contains(Marker))
                {
                    logger.LogWarning(
                        "index.html at {Path} already contains the splash marker but no backup file exists; skipping to avoid overwriting an unknown state",
                        indexPath);
                    return;
                }

                File.Copy(indexPath, backupPath);
            }

            // Always regenerate from the pristine backup rather than no-op'ing when the
            // marker is already present, so upgrading the plugin's injected snippet takes
            // effect on the next restart instead of leaving a stale patch in place forever.
            var original = File.ReadAllText(backupPath);
            var snippet = Marker + "\n<script defer src=\"/SplashScreen/loader.js\"></script>\n";
            var patched = Regex.Replace(original, "(</head>)", snippet + "$1", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

            if (File.ReadAllText(indexPath) != patched)
            {
                File.WriteAllText(indexPath, patched);
                logger.LogInformation("Injected splash screen loader into {Path}", indexPath);
            }
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
