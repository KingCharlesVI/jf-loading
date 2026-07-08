using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Jellyfin.Plugin.SplashScreen.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.SplashScreen.Controller;

[ApiController]
[Route("SplashScreen")]
public class SplashScreenController : ControllerBase
{
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".svg", ".webp" };

    [HttpGet("Config")]
    [AllowAnonymous]
    public ActionResult GetConfig()
    {
        var config = Plugin.Instance!.Configuration;

        string logoUrl = config.LogoMode == LogoSourceMode.Upload && !string.IsNullOrEmpty(config.UploadedLogoFileName)
            ? "/SplashScreen/Logo"
            : config.LogoUrl;

        return Ok(new
        {
            enabled = config.Enabled,
            logoUrl,
            backgroundColor = config.BackgroundColor,
            textColor = config.TextColor,
            progressBarColor = config.ProgressBarColor,
            loadingMessage = config.LoadingMessage,
            minimumDisplayDurationMs = config.MinimumDisplayDurationMs
        });
    }

    [HttpGet("Logo")]
    [AllowAnonymous]
    public ActionResult GetLogo()
    {
        var config = Plugin.Instance!.Configuration;
        if (string.IsNullOrEmpty(config.UploadedLogoFileName))
        {
            return NotFound();
        }

        var path = Path.Combine(GetLogoFolder(), config.UploadedLogoFileName);
        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        var contentType = Path.GetExtension(path).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return PhysicalFile(Path.GetFullPath(path), contentType);
    }

    [HttpPost("Logo")]
    [Authorize(Policy = "RequiresElevation")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult> UploadLogo(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return BadRequest("Unsupported file type.");
        }

        var folder = GetLogoFolder();
        Directory.CreateDirectory(folder);

        var config = Plugin.Instance!.Configuration;
        if (!string.IsNullOrEmpty(config.UploadedLogoFileName))
        {
            var previous = Path.Combine(folder, config.UploadedLogoFileName);
            if (System.IO.File.Exists(previous))
            {
                System.IO.File.Delete(previous);
            }
        }

        var fileName = "logo_" + DateTime.UtcNow.Ticks + extension;
        var path = Path.Combine(folder, fileName);

        await using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream);
        }

        config.UploadedLogoFileName = fileName;
        Plugin.Instance!.UpdateConfiguration(config);

        return Ok(new { fileName });
    }

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

    private static string GetLogoFolder()
    {
        return Plugin.Instance!.DataFolderPath;
    }
}
