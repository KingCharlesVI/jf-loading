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

## Install via repository URL (recommended)

1. In Jellyfin, go to Dashboard → Plugins → Repositories → **Add Repository**.
2. Set the URL to:
   ```
   https://raw.githubusercontent.com/KingCharlesVI/jf-loading/main/manifest.json
   ```
3. Go to Dashboard → Plugins → Catalog, find **Splash Screen**, and install it.
4. Restart the server, then configure it under Dashboard → Plugins → Splash Screen.

## Install (manual/local testing)

Copy the built DLL into your Jellyfin server's plugin folder, in its own subfolder named `Splash Screen_1.0.0.0` (matching `build.yaml`'s name/version), then restart the server:

```
<jellyfin data dir>/plugins/Splash Screen_1.0.0.0/Jellyfin.Plugin.SplashScreen.dll
```

Then open Dashboard → Plugins → Splash Screen to configure it.

## Cutting a release (maintainer)

Releases are built and published automatically by `.github/workflows/release.yml` whenever a matching git tag is pushed:

1. Bump the `version` field in `build.yaml` (must be four parts, e.g. `1.0.1.0`) and commit it to `main`.
2. Tag that commit to match, prefixed with `v`, and push the tag:
   ```
   git tag v1.0.1.0
   git push origin v1.0.1.0
   ```
3. CI will: build the plugin, create a GitHub Release for the tag with the plugin zip attached, then update and commit `manifest.json` on `main` with the new version entry (checksum + release asset URL), so existing installs pick up the update automatically.
