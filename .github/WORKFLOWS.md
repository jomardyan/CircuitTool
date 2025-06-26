# GitHub Actions Setup Guide

This repository uses GitHub Actions to automatically build, test, and publish the CircuitTool NuGet package. Here's how to set up and use the workflows.

## Workflows Overview

### 1. `.NET Build and Test` (dotnet.yml)
- **Triggers**: Push to main/develop branches, Pull requests
- **Purpose**: Builds and tests the project on multiple operating systems
- **Features**:
  - Cross-platform testing (Ubuntu, Windows, macOS)
  - Code analysis on pull requests
  - Test result artifacts

### 2. `Build and Publish NuGet Package` (publish-nuget.yml)
- **Triggers**: 
  - Release published
  - Tags starting with 'v*'
  - Manual dispatch with options
- **Purpose**: Builds, validates, and publishes NuGet packages
- **Features**:
  - Package validation
  - Dual publishing (GitHub Packages + NuGet.org)
  - Environment protection for production releases

## Required Secrets

To enable automatic publishing, you need to set up the following secrets in your GitHub repository:

### 1. NuGet.org API Key (`NUGET_API_KEY`)

1. Go to [NuGet.org](https://www.nuget.org)
2. Sign in and go to your profile
3. Click "API Keys" → "Create"
4. Configure the key:
   - **Key Name**: `CircuitTool-GitHub-Actions`
   - **Select Scopes**: `Push new packages and package versions`
   - **Select Packages**: Choose your package or use glob pattern
   - **Expiration**: Set appropriate expiration (recommended: 1 year)
5. Copy the generated API key
6. In your GitHub repository, go to Settings → Secrets and variables → Actions
7. Click "New repository secret"
8. Name: `NUGET_API_KEY`
9. Value: Paste your NuGet API key

### 2. GitHub Token (GITHUB_TOKEN)
This is automatically provided by GitHub Actions, no setup required.

## Environment Setup

### Production Environment
For additional security, the workflow uses a "production" environment for NuGet.org publishing:

1. Go to your repository Settings → Environments
2. Click "New environment"
3. Name: `production`
4. Configure protection rules:
   - ✅ Required reviewers (optional but recommended)
   - ✅ Wait timer (optional: 5 minutes)
   - ✅ Environment secrets (if you want environment-specific secrets)

## Publishing Workflow

### Automatic Publishing (Recommended)

1. **Create a Git Tag**:
   ```bash
   git tag v1.0.8
   git push origin v1.0.8
   ```

2. **Create a GitHub Release**:
   - Go to your repository → Releases → "Create a new release"
   - Choose the tag you created
   - Fill in release notes
   - Click "Publish release"

3. **Automatic Process**:
   - GitHub Actions will automatically build and test
   - Package will be validated
   - Published to GitHub Packages automatically
   - Published to NuGet.org automatically

### Manual Publishing

You can manually trigger publishing using the workflow dispatch:

1. Go to Actions → "Build and Publish NuGet Package"
2. Click "Run workflow"
3. Choose options:
   - ✅ Publish to GitHub Packages
   - ✅ Publish to NuGet.org (if ready for public release)

## Package Consumption

### From NuGet.org
```bash
dotnet add package CircuitTool
```

### From GitHub Packages

1. **Configure NuGet source**:
   ```bash
   dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"
   ```

2. **Install package**:
   ```bash
   dotnet add package CircuitTool --source github
   ```

## Versioning

The project uses the version specified in `CircuitTool.csproj`:
```xml
<Version>1.0.7</Version>
```

To release a new version:
1. Update the version in `CircuitTool.csproj`
2. Commit the change
3. Create a tag matching the version (e.g., `v1.0.8`)
4. Create a GitHub release

## Troubleshooting

### Common Issues

1. **NuGet API Key expired**:
   - Regenerate the key on NuGet.org
   - Update the `NUGET_API_KEY` secret in GitHub

2. **Package already exists**:
   - The workflow uses `--skip-duplicate` to handle this
   - Ensure you've incremented the version number

3. **Authentication failed for GitHub Packages**:
   - Ensure the `GITHUB_TOKEN` has proper permissions
   - Check that the repository visibility allows package publishing

4. **Build failures**:
   - Check the build logs in the Actions tab
   - Ensure all tests pass locally before pushing

### Support

For issues with the workflows:
1. Check the Actions tab for detailed logs
2. Ensure all required secrets are configured
3. Verify the package version has been incremented
4. Check that your NuGet API key has the correct permissions

## Security Best Practices

1. ✅ Use environment protection for production deployments
2. ✅ Regularly rotate API keys (annually recommended)
3. ✅ Use the principle of least privilege for API key scopes
4. ✅ Monitor package downloads and usage
5. ✅ Enable two-factor authentication on all accounts
