using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.SplashScreen.Controller;

[ApiController]
[Route("SplashScreen")]
public class SplashScreenController : ControllerBase
{
    [HttpGet("loader.js")]
    [AllowAnonymous]
    public ActionResult GetLoaderScript()
    {
        var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{typeof(Plugin).Namespace}.Web.loader.js");

        if (stream is null)
        {
            return NotFound();
        }

        return File(stream, "text/javascript");
    }

    [HttpGet("logo.svg")]
    [AllowAnonymous]
    public ActionResult GetLogo()
    {
        var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"{typeof(Plugin).Namespace}.Web.logo.svg");

        if (stream is null)
        {
            return NotFound();
        }

        return File(stream, "image/svg+xml");
    }
}
