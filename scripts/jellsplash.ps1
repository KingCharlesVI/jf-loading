<#
Tags and pushes a new plugin release.

Usage:
    jellsplash --v 1.0.0.6
    jellsplash -v 1.0.0.6
#>

$version = $null
for ($i = 0; $i -lt $args.Count; $i++) {
    if ($args[$i] -in @("--v", "-v", "--version")) {
        $version = $args[$i + 1]
        $i++
    }
}

if (-not $version) {
    Write-Error "Usage: jellsplash --v <version>  (e.g. jellsplash --v 1.0.0.6)"
    exit 1
}

if ($version -notmatch '^\d+\.\d+\.\d+\.\d+$') {
    Write-Error "Version must be four dot-separated numbers, e.g. 1.0.0.6"
    exit 1
}

$repoRoot = git rev-parse --show-toplevel 2>$null
if (-not $repoRoot) {
    Write-Error "Not inside a git repository."
    exit 1
}

Push-Location $repoRoot
try {
    $status = git status --porcelain
    if ($status) {
        Write-Error "Working tree has uncommitted changes. Commit or stash before tagging:`n$status"
        exit 1
    }

    $tag = "v$version"

    $existingTag = git tag -l $tag
    if ($existingTag) {
        Write-Error "Tag '$tag' already exists."
        exit 1
    }

    git tag $tag
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to create tag '$tag'."
        exit 1
    }

    git push origin main $tag
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to push tag '$tag'. Deleting local tag."
        git tag -d $tag | Out-Null
        exit 1
    }

    Write-Host "Pushed $tag. CI: https://github.com/KingCharlesVI/jf-loading/actions"
}
finally {
    Pop-Location
}
