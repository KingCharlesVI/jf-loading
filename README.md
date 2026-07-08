# jf-loading

A standalone Jellyfin server plugin that shows a custom loading/splash screen with a progress bar while the web client boots. Configurable from **Dashboard → Plugins → Splash Screen**: enable/disable, logo (URL or upload), background/text/progress-bar colors, loading message, and minimum display duration.

No dependency on any other plugin.

## How it works

On startup the plugin patches the server's `index.html` once (marker-guarded, idempotent, with an on-disk backup as `index.html.jf-loading-original`) to add a single `<script>` tag pointing at `/web/SplashScreen/loader.js`. That script fetches live settings from `/SplashScreen/Config` and renders the overlay, so changing settings in the dashboard takes effect immediately without re-patching anything.

## Build

```
dotnet build Jellyfin.Plugin.SplashScreen.slnx -c Release
```

Output DLL: `Jellyfin.Plugin.SplashScreen/bin/Release/net9.0/Jellyfin.Plugin.SplashScreen.dll`

## Install (manual/local testing)

Copy the built DLL into your Jellyfin server's plugin folder, in its own subfolder named `Splash Screen_1.0.0.0` (matching `build.yaml`'s name/version), then restart the server:

```
<jellyfin data dir>/plugins/Splash Screen_1.0.0.0/Jellyfin.Plugin.SplashScreen.dll
```

Then open Dashboard → Plugins → Splash Screen to configure it.
