# How to Obtain GitHub Personal Access Token

This guide will walk you through the process of creating a GitHub Personal Access Token (PAT) required for publishing packages to GitHub Packages.

## What is a Personal Access Token?

A Personal Access Token is a secure way to authenticate with GitHub's API and services without using your password. It acts as an alternative to password-based authentication and provides fine-grained control over permissions.

## Step-by-Step Instructions

### 1. Access GitHub Settings

1. **Sign in to GitHub** at https://github.com
2. **Click your profile picture** in the upper-right corner
3. **Select "Settings"** from the dropdown menu

### 2. Navigate to Developer Settings

1. **Scroll down** in the left sidebar
2. **Click "Developer settings"** at the bottom of the sidebar

### 3. Create Personal Access Token

1. **Click "Personal access tokens"** in the left sidebar
2. **Click "Tokens (classic)"** 
3. **Click "Generate new token"** button
4. **Select "Generate new token (classic)"**

### 4. Configure Token Settings

#### Basic Information
- **Note**: Give your token a descriptive name (e.g., "CircuitTool Package Publishing")
- **Expiration**: Choose an appropriate expiration time:
  - **30 days** - Good for testing
  - **90 days** - Good for regular use
  - **1 year** - For long-term projects
  - **No expiration** - Use with caution (security risk)

#### Required Scopes for GitHub Packages

Select the following scopes:

**For Publishing Packages:**
- ✅ **`write:packages`** - Upload packages to GitHub Package Registry
- ✅ **`read:packages`** - Download packages from GitHub Package Registry

**For Repository Access (if needed):**
- ✅ **`repo`** - Full control of private repositories (only if your repo is private)
- ✅ **`public_repo`** - Access public repositories (for public repos)

**Optional (for enhanced functionality):**
- ✅ **`workflow`** - Update GitHub Action workflows (if you plan to update workflows)
- ✅ **`admin:repo_hook`** - Full control of repository hooks

### 5. Generate and Save Token

1. **Click "Generate token"** at the bottom of the page
2. **Copy the token immediately** - You won't be able to see it again!
3. **Store the token securely** in a password manager or secure location

⚠️ **Important**: The token will only be displayed once. If you lose it, you'll need to generate a new one.

## Using Your Token

### For Command Line Publishing

Replace `YOUR_GITHUB_TOKEN` with your actual token:

```bash
# Add GitHub Packages source
dotnet nuget add source --username YOUR_GITHUB_USERNAME --password YOUR_GITHUB_TOKEN --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"

# Publish package
dotnet nuget push "bin/Release/CircuitTool.1.0.5.nupkg" --source "github"
```

### For Scripts

#### PowerShell (Windows)
```powershell
# Set environment variable
$env:GITHUB_TOKEN = "your_token_here"

# Use the script
.\publish.ps1 -GitHub
```

#### Bash (Linux/macOS)
```bash
# Set environment variable
export GITHUB_TOKEN="your_token_here"

# Use the script
./publish.sh --github
```

### For GitHub Actions

1. **Go to your repository on GitHub**
2. **Click "Settings" tab**
3. **Click "Secrets and variables" > "Actions"**
4. **Click "New repository secret"**
5. **Name**: `GITHUB_TOKEN` (this is automatically provided) or `NUGET_API_KEY` (for NuGet.org)
6. **Value**: Your token
7. **Click "Add secret"**

## Security Best Practices

### ✅ Do's
- **Use descriptive names** for your tokens
- **Set appropriate expiration dates** 
- **Use the minimum required scopes**
- **Store tokens securely** (password manager, environment variables)
- **Rotate tokens regularly**
- **Delete unused tokens**

### ❌ Don'ts
- **Never commit tokens to code repositories**
- **Don't share tokens in plain text**
- **Don't use tokens with excessive permissions**
- **Don't store tokens in unsecured locations**

## Troubleshooting

### Common Issues

**"401 Unauthorized" Error:**
- Check if token has correct scopes (`write:packages`, `read:packages`)
- Verify token hasn't expired
- Ensure username is correct in the command

**"403 Forbidden" Error:**
- Check if you have write access to the repository
- Verify the package name matches the repository owner
- Ensure the repository allows package publishing

**Token Not Working:**
- Generate a new token
- Double-check the scopes are selected correctly
- Verify you copied the complete token (no spaces or truncation)

### Testing Your Token

You can test if your token works by listing your packages:

```bash
curl -H "Authorization: token YOUR_GITHUB_TOKEN" \
  https://api.github.com/user/packages?package_type=nuget
```

## Token Management

### Viewing Existing Tokens
1. Go to **GitHub Settings** > **Developer settings** > **Personal access tokens** > **Tokens (classic)**
2. View all your active tokens, their scopes, and expiration dates

### Revoking Tokens
1. Find the token in your token list
2. Click **"Delete"** next to the token
3. Confirm deletion

### Updating Token Scopes
You cannot modify existing tokens. You must:
1. Create a new token with the desired scopes
2. Update your applications/scripts with the new token
3. Delete the old token

## Alternative: Fine-grained Personal Access Tokens (Beta)

GitHub also offers fine-grained tokens that provide more specific repository access:

1. Go to **Personal access tokens** > **Fine-grained tokens**
2. Click **"Generate new token"**
3. Select specific repositories and permissions
4. This provides more granular control but is currently in beta

---

## Quick Reference

**GitHub Packages Token Requirements:**
- Scopes: `write:packages`, `read:packages`
- Registry URL: `https://nuget.pkg.github.com/OWNER/index.json`
- Authentication: Username + Token

**Links:**
- [GitHub PAT Documentation](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token)
- [GitHub Packages Documentation](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-nuget-registry)
