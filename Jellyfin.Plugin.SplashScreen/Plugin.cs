using System;
using System.Collections.Generic;
using Jellyfin.Plugin.SplashScreen.Configuration;
using Jellyfin.Plugin.SplashScreen.Injection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.SplashScreen;

public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(
        IApplicationPaths applicationPaths,
        IXmlSerializer xmlSerializer,
        ILogger<Plugin> logger)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;

        try
        {
            SplashInjector.Apply(applicationPaths.WebPath, logger);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to inject splash screen loader into index.html");
        }
    }

    public static Plugin? Instance { get; private set; }

    public override string Name => "Splash Screen";

    public override Guid Id => Guid.Parse("045bee24-5e88-4376-a9e3-0e259eceae73");

    public IEnumerable<PluginPageInfo> GetPages()
    {
        yield return new PluginPageInfo
        {
            Name = Name,
            EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html"
        };
    }
}
