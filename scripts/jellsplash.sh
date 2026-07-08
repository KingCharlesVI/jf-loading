#!/usr/bin/env bash
# Tags and pushes a new plugin release.
#
# Usage:
#   jellsplash --v 1.0.0.6
#   jellsplash -v 1.0.0.6

set -euo pipefail

version=""
while [[ $# -gt 0 ]]; do
  case "$1" in
    --v|-v|--version)
      version="${2:-}"
      shift 2
      ;;
    *)
      echo "Unknown argument: $1" >&2
      exit 1
      ;;
  esac
done

if [[ -z "$version" ]]; then
  echo "Usage: jellsplash --v <version>  (e.g. jellsplash --v 1.0.0.6)" >&2
  exit 1
fi

if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
  echo "Version must be four dot-separated numbers, e.g. 1.0.0.6" >&2
  exit 1
fi

repo_root=$(git rev-parse --show-toplevel)
cd "$repo_root"

if [[ -n "$(git status --porcelain)" ]]; then
  echo "Working tree has uncommitted changes. Commit or stash before tagging:" >&2
  git status --short >&2
  exit 1
fi

tag="v$version"

if git rev-parse "$tag" >/dev/null 2>&1; then
  echo "Tag '$tag' already exists." >&2
  exit 1
fi

git tag "$tag"
if ! git push origin main "$tag"; then
  echo "Failed to push tag '$tag'. Deleting local tag." >&2
  git tag -d "$tag" >/dev/null
  exit 1
fi

echo "Pushed $tag. CI: https://github.com/KingCharlesVI/jf-loading/actions"
