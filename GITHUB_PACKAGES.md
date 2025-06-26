# Publishing to GitHub Packages

This document explains how to publish CircuitTool to GitHub Packages.

## Prerequisites

1. **GitHub Personal Access Token (PAT)** with `write:packages` and `read:packages` permissions
2. **Package name** must be scoped to your GitHub username or organization

## Setup Instructions

### 1. Create a Personal Access Token

1. Go to GitHub Settings > Developer settings > Personal access tokens > Tokens (classic)
2. Click "Generate new token (classic)"
3. Select the following scopes:
   - `write:packages` - Upload packages to GitHub Package Registry
   - `read:packages` - Download packages from GitHub Package Registry
   - `repo` - Full control of private repositories (if needed)

**📖 For detailed step-by-step instructions with screenshots, see [GITHUB_TOKEN_GUIDE.md](GITHUB_TOKEN_GUIDE.md)**

### 2. Configure NuGet for GitHub Packages

Add GitHub Packages as a package source:

```bash
# Add GitHub Packages source
dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"
```

### 3. Publish to GitHub Packages

```bash
# Build and pack the project
dotnet pack --configuration Release

# Publish to GitHub Packages
dotnet nuget push "bin/Release/CircuitTool.1.0.5.nupkg" --source "github"
```

### 4. Alternative: Using GitHub Actions

See `.github/workflows/publish-github-packages.yml` for automated publishing.

## Package Information

- **Package ID**: `CircuitTool`
- **GitHub Packages URL**: `https://github.com/jomardyan/CircuitTool/packages`
- **NuGet Source URL**: `https://nuget.pkg.github.com/jomardyan/index.json`

## Installing from GitHub Packages

To install the package from GitHub Packages:

```bash
# Add GitHub Packages source (one-time setup)
dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"

# Install the package
dotnet add package CircuitTool --source github
```

## Security Notes

- Store your GitHub token securely
- Consider using GitHub Actions secrets for automated publishing
- Use read-only tokens for package consumption when possible
