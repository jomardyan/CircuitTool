#!/bin/bash

# CircuitTool Publishing Script for Linux/macOS
# This script helps publish CircuitTool to both GitHub Packages and NuGet.org

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Default values
PUBLISH_GITHUB=false
PUBLISH_NUGET=false
PUBLISH_ALL=false
GITHUB_TOKEN=""
NUGET_API_KEY=""

# Function to print colored output
print_color() {
    printf "${2}${1}${NC}\n"
}

# Function to show usage
show_usage() {
    echo "CircuitTool Publishing Script"
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  -g, --github              Publish to GitHub Packages"
    echo "  -n, --nuget               Publish to NuGet.org"
    echo "  -a, --all                 Publish to both GitHub Packages and NuGet.org"
    echo "  -t, --github-token TOKEN  GitHub Personal Access Token"
    echo "  -k, --nuget-key KEY       NuGet.org API Key"
    echo "  -h, --help                Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 --github"
    echo "  $0 --nuget"
    echo "  $0 --all"
    echo "  $0 --github --github-token 'your-token'"
    echo ""
    echo "Environment Variables:"
    echo "  GITHUB_TOKEN              GitHub Personal Access Token"
    echo "  NUGET_API_KEY             NuGet.org API Key"
}

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -g|--github)
            PUBLISH_GITHUB=true
            shift
            ;;
        -n|--nuget)
            PUBLISH_NUGET=true
            shift
            ;;
        -a|--all)
            PUBLISH_ALL=true
            shift
            ;;
        -t|--github-token)
            GITHUB_TOKEN="$2"
            shift 2
            ;;
        -k|--nuget-key)
            NUGET_API_KEY="$2"
            shift 2
            ;;
        -h|--help)
            show_usage
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            show_usage
            exit 1
            ;;
    esac
done

# Function to check prerequisites
check_prerequisites() {
    print_color "Checking prerequisites..." "$BLUE"
    
    # Check if dotnet is installed
    if ! command -v dotnet &> /dev/null; then
        print_color "✗ .NET SDK not found. Please install .NET SDK." "$RED"
        exit 1
    fi
    
    local dotnet_version=$(dotnet --version)
    print_color "✓ .NET SDK version: $dotnet_version" "$GREEN"
    
    # Check if project file exists
    if [ ! -f "CircuitTool.csproj" ]; then
        print_color "✗ CircuitTool.csproj not found. Run this script from the project root." "$RED"
        exit 1
    fi
    
    print_color "✓ Project file found" "$GREEN"
}

# Function to build project
build_project() {
    print_color "Building project..." "$BLUE"
    
    # Restore dependencies
    print_color "Restoring dependencies..." "$YELLOW"
    dotnet restore
    
    # Build project
    print_color "Building project..." "$YELLOW"
    dotnet build --configuration Release --no-restore
    
    # Run tests
    print_color "Running tests..." "$YELLOW"
    dotnet test --configuration Release --no-build --verbosity minimal
    
    # Create packages directory
    mkdir -p packages
    
    # Pack project
    print_color "Creating NuGet package..." "$YELLOW"
    dotnet pack --configuration Release --no-build --output ./packages
    
    print_color "✓ Build completed successfully" "$GREEN"
}

# Function to publish to GitHub Packages
publish_to_github() {
    local token="$1"
    
    print_color "Publishing to GitHub Packages..." "$BLUE"
    
    if [ -z "$token" ]; then
        token="$GITHUB_TOKEN"
        if [ -z "$token" ]; then
            print_color "✗ GitHub token not provided. Set GITHUB_TOKEN environment variable or use --github-token parameter." "$RED"
            return 1
        fi
    fi
    
    # Remove existing GitHub source if it exists (to avoid duplicate warning)
    dotnet nuget remove source github 2>/dev/null || true
    
    # Add GitHub Packages source
    dotnet nuget add source --username "jomardyan" --password "$token" --store-password-in-clear-text --name github "https://nuget.pkg.github.com/jomardyan/index.json"
    
    # Set API key for GitHub Packages source
    dotnet nuget setapikey "$token" --source github
    
    # Push to GitHub Packages
    for package in ./packages/*.nupkg; do
        if [ -f "$package" ]; then
            print_color "Publishing $(basename "$package")..." "$YELLOW"
            dotnet nuget push "$package" --source "github" --api-key "$token" --skip-duplicate
        fi
    done
    
    print_color "✓ Successfully published to GitHub Packages" "$GREEN"
    return 0
}

# Function to publish to NuGet.org
publish_to_nuget() {
    local api_key="$1"
    
    print_color "Publishing to NuGet.org..." "$BLUE"
    
    if [ -z "$api_key" ]; then
        api_key="$NUGET_API_KEY"
        if [ -z "$api_key" ]; then
            print_color "✗ NuGet API key not provided. Set NUGET_API_KEY environment variable or use --nuget-key parameter." "$RED"
            return 1
        fi
    fi
    
    # Push to NuGet.org
    for package in ./packages/*.nupkg; do
        if [ -f "$package" ]; then
            print_color "Publishing $(basename "$package")..." "$YELLOW"
            dotnet nuget push "$package" --api-key "$api_key" --source https://api.nuget.org/v3/index.json --skip-duplicate
        fi
    done
    
    print_color "✓ Successfully published to NuGet.org" "$GREEN"
    return 0
}

# Main execution
print_color "CircuitTool Publishing Script" "$BLUE"
print_color "==============================" "$BLUE"

check_prerequisites
build_project

success=true

if [ "$PUBLISH_ALL" = true ] || [ "$PUBLISH_GITHUB" = true ]; then
    if ! publish_to_github "$GITHUB_TOKEN"; then
        success=false
    fi
fi

if [ "$PUBLISH_ALL" = true ] || [ "$PUBLISH_NUGET" = true ]; then
    if ! publish_to_nuget "$NUGET_API_KEY"; then
        success=false
    fi
fi

if [ "$PUBLISH_ALL" = false ] && [ "$PUBLISH_GITHUB" = false ] && [ "$PUBLISH_NUGET" = false ]; then
    print_color "No publishing target specified. Use --github, --nuget, or --all" "$YELLOW"
    show_usage
fi

if [ "$success" = true ]; then
    print_color "✓ All publishing operations completed successfully!" "$GREEN"
else
    print_color "✗ Some publishing operations failed." "$RED"
    exit 1
fi
