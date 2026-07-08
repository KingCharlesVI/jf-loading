# Changelog

All notable changes to the Splash Screen plugin are documented here, per released version.

## Unreleased

- Removed all configuration. The splash screen is now fixed: black background, a large logo, and a white progress indicator, shown for a minimum of 3 seconds. Removed the settings page, the sidebar entry, plugin configuration storage, and the logo upload/URL/color/message/duration options entirely.
- The logo is now a fixed embedded resource (`Web/logo.svg`) served at `/SplashScreen/logo.svg`, replacing the previous per-install URL/upload options.

## 1.0.0.6

- Fixed settings not saving: `LogoMode` was a C# enum, which `System.Text.Json` cannot deserialize from a plain string without an explicit converter, so every save silently failed server-side. Changed to a plain string field.
- Fixed the config page fully reloading instead of saving via AJAX: the `change`/`click`/`submit` listeners were attached at top-level script scope, before Jellyfin had necessarily finished mounting the page's inner elements, so a `null` from an early `querySelector` could throw and abort the rest of the script before the submit listener was ever attached. All interactive listeners now attach inside the `pageshow` callback instead, matching the pattern used by other working Jellyfin plugins.
- Config page now surfaces save failures as a visible alert instead of failing silently.

## 1.0.0.5

- Fixed the splash screen never loading: the injected `<script>` tag pointed at `/web/SplashScreen/loader.js`, but plugin controllers are mounted at the root (`/SplashScreen/...`), not under `/web/` (that prefix is reserved for the static web client files).
- Made `index.html` injection self-correcting: previously, once the marker comment was present the injector would never touch the file again, so a fixed injection snippet could never reach an already-patched install. It now always regenerates the snippet from the pristine backup on every startup and only rewrites the file if something actually changed.
- Fixed the plugin not appearing in the Dashboard sidebar: `PluginPageInfo.EnableInMainMenu` defaults to `false` and was never set, so the config page was reachable directly by URL but never listed.

## 1.0.0.4

- Fixed the installed plugin version always showing `1.0.0.0` regardless of the release tag: the build pipeline only reads `version` from `build.yaml` unless explicitly overridden. The release workflow now derives the version from the pushed git tag and passes it through to the build, so `build.yaml` no longer needs to be bumped manually for each release.

## 1.0.0.0

Initial functional release.

- Splash/loading screen with a progress bar, shown while the Jellyfin web client boots, injected into `index.html` at server startup (idempotent, marker-guarded, with an on-disk backup for safe reverts).
- Configurable via Dashboard → Plugins → Splash Screen: enable/disable, logo (URL or upload), background/text/progress-bar colors, loading message, minimum display duration.
- Installable via a repository URL (`manifest.json`), with a GitHub Actions workflow that builds, releases, and updates the manifest automatically on tag push.

### Release pipeline fixes along the way to 1.0.0.0

These didn't change plugin behavior but were necessary to get releases publishing at all:

- `jprm plugin build` looks for `build.yaml` inside the given project path, not the repo root — moved it into `Jellyfin.Plugin.SplashScreen/`.
- Added `.gitignore` for `bin/`/`obj/`, which had been committed directly.
- `jprm repo add manifest.json ...` failed on a bare filename (`os.path.dirname("manifest.json")` is `""`, and `os.path.exists("")` is always `False`) — changed to `./manifest.json`.
