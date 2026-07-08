# jf-loading

A standalone Jellyfin server plugin that shows a loading/splash screen while the web client boots: a black screen with a large logo and a white progress indicator, visible for a minimum of 3 seconds.

There is no configuration — the appearance is fixed. No dependency on any other plugin.

See [CHANGELOG.md](CHANGELOG.md) for release history.

## How it works

On startup the plugin patches the server's `index.html` (marker-guarded, idempotent, with an on-disk backup as `index.html.jf-loading-original`, regenerated from that backup on every startup so plugin upgrades take effect) to add a single `<script>` tag pointing at `/SplashScreen/loader.js`. That script renders the overlay directly (no server round-trip beyond loading the logo image) and removes itself once the web client is ready and at least 3 seconds have elapsed.

The logo is `Jellyfin.Plugin.SplashScreen/Web/logo.svg`, bundled into the plugin as an embedded resource and served at `/SplashScreen/logo.svg`. Replace that file to change the logo; no other customization points exist.

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
4. Restart the server. Nothing to configure — the splash screen is active immediately.

## Install (manual/local testing)

Copy the built DLL into your Jellyfin server's plugin folder, in its own subfolder named `Splash Screen_<version>` (matching the built assembly version), then restart the server:

```
<jellyfin data dir>/plugins/Splash Screen_1.0.0.0/Jellyfin.Plugin.SplashScreen.dll
```

Nothing to configure — restart and it's active.

## Cutting a release (maintainer)

Releases are built and published automatically by `.github/workflows/release.yml` whenever a matching git tag is pushed. The tag itself is the source of truth for the version (the workflow strips the leading `v` and passes it to `dotnet publish -p:Version=`), so `build.yaml`'s `version` field does not need to be bumped manually:

```
git tag v1.0.1.0
git push origin v1.0.1.0
```

CI will: build the plugin with that version, create a GitHub Release for the tag with the plugin zip attached, then update and commit `manifest.json` on `main` with the new version entry (checksum + release asset URL), so existing installs pick up the update automatically.
