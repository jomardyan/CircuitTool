# Documentation generation script for CircuitTool
# This script installs DocFX and generates HTML documentation

param(
    [switch]$Install,
    [switch]$Build,
    [switch]$Serve,
    [switch]$Clean,
    [switch]$Open,
    [switch]$Auto,
    [string]$OutputPath = "docs/_site"
)

$ErrorActionPreference = "Stop"

Write-Host "CircuitTool Documentation Generator" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan

# Check if DocFX is installed
function Test-DocFXInstalled {
    try {
        $version = docfx --version 2>$null
        if ($version) {
            Write-Host "✓ DocFX is already installed: $version" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "✗ DocFX not found" -ForegroundColor Yellow
        return $false
    }
    return $false
}

# Install DocFX
function Install-DocFX {
    Write-Host "Installing DocFX..." -ForegroundColor Yellow
    
    if (Get-Command dotnet -ErrorAction SilentlyContinue) {
        Write-Host "Installing DocFX as a global .NET tool..." -ForegroundColor Blue
        dotnet tool install -g docfx
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ DocFX installed successfully!" -ForegroundColor Green
        } else {
            Write-Host "✗ Failed to install DocFX via dotnet tool" -ForegroundColor Red
            
            # Fallback: Try to download DocFX manually
            Write-Host "Trying alternative installation method..." -ForegroundColor Yellow
            
            $docfxUrl = "https://github.com/dotnet/docfx/releases/download/v2.75.3/docfx-win-x64-v2.75.3.zip"
            $docfxZip = "docfx.zip"
            $docfxDir = "docfx"
            
            Invoke-WebRequest -Uri $docfxUrl -OutFile $docfxZip
            Expand-Archive -Path $docfxZip -DestinationPath $docfxDir -Force
            Remove-Item $docfxZip
            
            # Add to PATH temporarily
            $env:PATH += ";$PWD\$docfxDir"
            
            Write-Host "✓ DocFX downloaded and extracted!" -ForegroundColor Green
        }
    } else {
        Write-Host "✗ .NET CLI not found. Please install .NET SDK first." -ForegroundColor Red
        exit 1
    }
}

# Build documentation
function Build-Documentation {
    Write-Host "Building documentation..." -ForegroundColor Yellow
    
    if (!(Test-Path "config/docfx.json")) {
        Write-Host "✗ config/docfx.json not found. Make sure you're in the project root directory." -ForegroundColor Red
        exit 1
    }
    
    # Build the project first to generate XML documentation
    Write-Host "Building CircuitTool project..." -ForegroundColor Blue
    dotnet build --configuration Release
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ Project build failed" -ForegroundColor Red
        exit 1
    }
    
    # Generate API documentation
    Write-Host "Generating API documentation..." -ForegroundColor Blue
    docfx metadata config/docfx.json
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "✗ API documentation generation failed" -ForegroundColor Red
        exit 1
    }
    
    # Build HTML documentation
    Write-Host "Building HTML documentation..." -ForegroundColor Blue
    docfx build config/docfx.json
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Documentation built successfully!" -ForegroundColor Green
        Write-Host "Documentation available at: $OutputPath/index.html" -ForegroundColor Cyan
    } else {
        Write-Host "✗ Documentation build failed" -ForegroundColor Red
        exit 1
    }
}

# Serve documentation locally
function Serve-Documentation {
    Write-Host "Starting documentation server..." -ForegroundColor Yellow
    
    if (!(Test-Path "$OutputPath/index.html")) {
        Write-Host "Documentation not found. Building first..." -ForegroundColor Yellow
        Build-Documentation
    }
    
    Write-Host "Documentation server starting at http://localhost:8080" -ForegroundColor Cyan
    Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
    
    docfx serve $OutputPath --port 8080
}

# Clean generated files
function Clean-Documentation {
    Write-Host "Cleaning generated documentation files..." -ForegroundColor Yellow
    
    $pathsToClean = @("_site", "api", "obj")
    
    foreach ($path in $pathsToClean) {
        if (Test-Path $path) {
            Remove-Item $path -Recurse -Force
            Write-Host "✓ Cleaned $path" -ForegroundColor Green
        }
    }
    
    Write-Host "✓ Cleanup completed!" -ForegroundColor Green
}

# Main script logic
if ($Install) {
    if (!(Test-DocFXInstalled)) {
        Install-DocFX
    }
} elseif ($Build) {
    if (!(Test-DocFXInstalled)) {
        Write-Host "DocFX not installed. Installing first..." -ForegroundColor Yellow
        Install-DocFX
    }
    Build-Documentation
} elseif ($Serve) {
    if (!(Test-DocFXInstalled)) {
        Write-Host "DocFX not installed. Installing first..." -ForegroundColor Yellow
        Install-DocFX
    }
    Serve-Documentation
} elseif ($Clean) {
    Clean-Documentation
} else {
    # Default: Install, build, and serve
    if (!(Test-DocFXInstalled)) {
        Install-DocFX
    }
    Build-Documentation
    
    Write-Host ""
    Write-Host "Would you like to start the documentation server? (y/n)" -ForegroundColor Cyan
    $response = Read-Host
    if ($response -eq 'y' -or $response -eq 'Y' -or $response -eq 'yes') {
        Serve-Documentation
    } else {
        Write-Host "Documentation is ready at: $OutputPath/index.html" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Documentation generation completed!" -ForegroundColor Green
