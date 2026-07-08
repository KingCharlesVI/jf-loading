using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

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

        // This is fetched by a static <script> tag injected once into index.html, so
        // its URL never changes between plugin versions. Without this, browsers can
        // keep serving a stale cached copy indefinitely after an upgrade.
        Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store, must-revalidate";
        Response.Headers[HeaderNames.Pragma] = "no-cache";
        Response.Headers[HeaderNames.Expires] = "0";

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
