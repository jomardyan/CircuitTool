# CircuitTool Publishing Script
# This script helps publish CircuitTool to both GitHub Packages and NuGet.org

param(
    [Parameter(Mandatory=$false)]
    [string]$Version = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$GitHub = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$NuGet = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$All = $false,
    
    [Parameter(Mandatory=$false)]
    [string]$GitHubToken = "",
    
    [Parameter(Mandatory=$false)]
    [string]$NuGetApiKey = ""
)

# Colors for output
$Green = "Green"
$Red = "Red"
$Yellow = "Yellow"
$Blue = "Blue"

function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Test-Prerequisites {
    Write-ColorOutput "Checking prerequisites..." $Blue
    
    # Check if dotnet is installed
    try {
        $dotnetVersion = dotnet --version
        Write-ColorOutput "✓ .NET SDK version: $dotnetVersion" $Green
    }
    catch {
        Write-ColorOutput "✗ .NET SDK not found. Please install .NET SDK." $Red
        exit 1
    }
    
    # Check if project file exists
    if (-not (Test-Path "CircuitTool.csproj")) {
        Write-ColorOutput "✗ CircuitTool.csproj not found. Run this script from the project root." $Red
        exit 1
    }
    
    Write-ColorOutput "✓ Project file found" $Green
}

function Build-Project {
    Write-ColorOutput "Building project..." $Blue
    
    # Restore dependencies
    Write-ColorOutput "Restoring dependencies..." $Yellow
    dotnet restore
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "✗ Failed to restore dependencies" $Red
        exit 1
    }
    
    # Build project
    Write-ColorOutput "Building project..." $Yellow
    dotnet build --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "✗ Build failed" $Red
        exit 1
    }
    
    # Run tests
    Write-ColorOutput "Running tests..." $Yellow
    dotnet test --configuration Release --no-build --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "✗ Tests failed" $Red
        exit 1
    }
    
    # Pack project
    Write-ColorOutput "Creating NuGet package..." $Yellow
    dotnet pack --configuration Release --no-build --output ./packages
    if ($LASTEXITCODE -ne 0) {
        Write-ColorOutput "✗ Pack failed" $Red
        exit 1
    }
    
    Write-ColorOutput "✓ Build completed successfully" $Green
}

function Publish-ToGitHub {
    param([string]$Token)
    
    Write-ColorOutput "Publishing to GitHub Packages..." $Blue
    
    if ([string]::IsNullOrEmpty($Token)) {
        $Token = $env:GITHUB_TOKEN
        if ([string]::IsNullOrEmpty($Token)) {
            Write-ColorOutput "✗ GitHub token not provided. Set GITHUB_TOKEN environment variable or use -GitHubToken parameter." $Red
            return $false
        }
    }
    
    # Remove existing GitHub source if it exists (to avoid duplicate warning)
    try {
        dotnet nuget remove source github 2>$null
    } catch {
        # Ignore error if source doesn't exist
    }
    
    # Add GitHub Packages source
    dotnet nuget add source --username "jomardyan" --password $Token --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"
    
    # Set API key for GitHub Packages source
    dotnet nuget setapikey $Token --source github
    
    # Push to GitHub Packages
    $packages = Get-ChildItem -Path "./packages/*.nupkg"
    foreach ($package in $packages) {
        Write-ColorOutput "Publishing $($package.Name)..." $Yellow
        dotnet nuget push $package.FullName --source "github" --api-key $Token --skip-duplicate
        if ($LASTEXITCODE -ne 0) {
            Write-ColorOutput "✗ Failed to publish $($package.Name) to GitHub Packages" $Red
            return $false
        }
    }
    
    Write-ColorOutput "✓ Successfully published to GitHub Packages" $Green
    return $true
}

function Publish-ToNuGet {
    param([string]$ApiKey)
    
    Write-ColorOutput "Publishing to NuGet.org..." $Blue
    
    if ([string]::IsNullOrEmpty($ApiKey)) {
        $ApiKey = $env:NUGET_API_KEY
        if ([string]::IsNullOrEmpty($ApiKey)) {
            Write-ColorOutput "✗ NuGet API key not provided. Set NUGET_API_KEY environment variable or use -NuGetApiKey parameter." $Red
            return $false
        }
    }
    
    # Push to NuGet.org
    $packages = Get-ChildItem -Path "./packages/*.nupkg"
    foreach ($package in $packages) {
        Write-ColorOutput "Publishing $($package.Name)..." $Yellow
        dotnet nuget push $package.FullName --api-key $ApiKey --source https://api.nuget.org/v3/index.json --skip-duplicate
        if ($LASTEXITCODE -ne 0) {
            Write-ColorOutput "✗ Failed to publish $($package.Name) to NuGet.org" $Red
            return $false
        }
    }
    
    Write-ColorOutput "✓ Successfully published to NuGet.org" $Green
    return $true
}

# Main execution
Write-ColorOutput "CircuitTool Publishing Script" $Blue
Write-ColorOutput "==============================" $Blue

Test-Prerequisites
Build-Project

$success = $true

if ($All -or $GitHub) {
    $result = Publish-ToGitHub -Token $GitHubToken
    $success = $success -and $result
}

if ($All -or $NuGet) {
    $result = Publish-ToNuGet -ApiKey $NuGetApiKey
    $success = $success -and $result
}

if (-not ($All -or $GitHub -or $NuGet)) {
    Write-ColorOutput "No publishing target specified. Use -GitHub, -NuGet, or -All" $Yellow
    Write-ColorOutput "Usage examples:" $Blue
    Write-ColorOutput "  .\publish.ps1 -GitHub" $Blue
    Write-ColorOutput "  .\publish.ps1 -NuGet" $Blue
    Write-ColorOutput "  .\publish.ps1 -All" $Blue
    Write-ColorOutput "  .\publish.ps1 -GitHub -GitHubToken 'your-token'" $Blue
}

if ($success) {
    Write-ColorOutput "✓ All publishing operations completed successfully!" $Green
} else {
    Write-ColorOutput "✗ Some publishing operations failed." $Red
    exit 1
}
