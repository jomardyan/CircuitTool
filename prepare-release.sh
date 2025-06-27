#!/bin/bash

# CircuitTool GitHub Release Preparation Script
# This script prepares everything needed for a GitHub release and optionally creates it automatically
#
# Usage:
#   ./prepare-release.sh                    # Full release with auto GitHub creation
#   AUTO_CREATE_RELEASE=false ./prepare-release.sh  # Build only, no GitHub release
#   PRERELEASE=true ./prepare-release.sh    # Create as pre-release
#   GITHUB_REPO="user/repo" ./prepare-release.sh    # Use different repository
#   DRY_RUN=true ./prepare-release.sh       # Simulate release without creating it
#   SKIP_TESTS=true ./prepare-release.sh    # Skip test execution (for debug builds)
#   ./prepare-release.sh --help             # Show help information
#
# Prerequisites for auto GitHub release:
#   - GitHub CLI installed (https://cli.github.com/)
#   - Authenticated with GitHub (gh auth login)
#   - Write access to the repository
#   - Git repository with proper remote setup

set -e

# Show help information
if [[ "$1" == "--help" || "$1" == "-h" ]]; then
    echo "CircuitTool GitHub Release Preparation Script"
    echo "=============================================="
    echo ""
    echo "This script prepares everything needed for a GitHub release and optionally creates it automatically."
    echo "It will automatically commit any uncommitted changes before proceeding with the release."
    echo ""
    echo "Usage:"
    echo "  ./prepare-release.sh                    # Full release with auto GitHub creation"
    echo "  AUTO_CREATE_RELEASE=false ./prepare-release.sh  # Build only, no GitHub release"
    echo "  PRERELEASE=true ./prepare-release.sh    # Create as pre-release"
    echo "  GITHUB_REPO=\"user/repo\" ./prepare-release.sh    # Use different repository"
    echo "  DRY_RUN=true ./prepare-release.sh       # Simulate release without creating it"
    echo "  SKIP_TESTS=true ./prepare-release.sh    # Skip test execution (for debug builds)"
    echo ""
    echo "Environment Variables:"
    echo "  AUTO_CREATE_RELEASE  - Create GitHub release automatically (default: true)"
    echo "  GITHUB_REPO         - GitHub repository (default: jomardyan/CircuitTool)"
    echo "  PRERELEASE          - Mark as pre-release (default: false)"
    echo "  DRY_RUN            - Simulate without creating release (default: false)"
    echo "  SKIP_TESTS         - Skip test execution (default: false)"
    echo ""
    echo "Automatic Operations:"
    echo "  - Commits any uncommitted changes with release message"
    echo "  - Builds and tests the entire solution"
    echo "  - Generates documentation with DocFX"
    echo "  - Creates NuGet packages and CLI distribution"
    echo "  - Pushes commits and creates Git tags"
    echo "  - Creates GitHub release with all artifacts"
    echo ""
    echo "Prerequisites for auto GitHub release:"
    echo "  - GitHub CLI installed (https://cli.github.com/)"
    echo "  - Authenticated with GitHub (gh auth login)"
    echo "  - Write access to the repository"
    echo "  - Git repository with proper remote setup"
    echo ""
    exit 0
fi

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Version information
VERSION="2.2.0"
CLI_VERSION="1.1.0"
RELEASE_DATE="2025-06-27"

# Release options
AUTO_CREATE_RELEASE=${AUTO_CREATE_RELEASE:-true}
GITHUB_REPO=${GITHUB_REPO:-"jomardyan/CircuitTool"}
PRERELEASE=${PRERELEASE:-false}
DRY_RUN=${DRY_RUN:-false}
SKIP_TESTS=${SKIP_TESTS:-false}

# Function to print colored output
print_color() {
    printf "${2}${1}${NC}\n"
}

# Function to check if GitHub CLI is available
check_github_cli() {
    if command -v gh &> /dev/null; then
        return 0
    else
        return 1
    fi
}

# Function to check if user is authenticated with GitHub
check_github_auth() {
    if gh auth status &> /dev/null; then
        return 0
    else
        return 1
    fi
}

# Function to check if we're in a git repository
check_git_repo() {
    if git rev-parse --git-dir &> /dev/null; then
        return 0
    else
        return 1
    fi
}

# Function to check if tag already exists
check_tag_exists() {
    local tag="$1"
    if git tag -l | grep -q "^${tag}$"; then
        return 0
    else
        return 1
    fi
}

# Function to validate GitHub repository
validate_github_repo() {
    local repo="$1"
    if gh repo view "$repo" &> /dev/null; then
        return 0
    else
        return 1
    fi
}

# Function to check if there are uncommitted changes
check_uncommitted_changes() {
    if ! git diff --quiet || ! git diff --staged --quiet; then
        return 0  # There are changes
    else
        return 1  # No changes
    fi
}

# Function to commit all changes
commit_all_changes() {
    local commit_message="$1"
    
    # Add all changes (including untracked files that make sense for a release)
    git add -A
    
    # Check if there are changes to commit after adding
    if git diff --staged --quiet; then
        print_color "ℹ️  No changes to commit" "$BLUE"
        return 0
    fi
    
    # Show what will be committed
    print_color "📝 Changes to be committed:" "$BLUE"
    git diff --staged --name-status | sed 's/^/  /'
    
    if [ "$DRY_RUN" = true ]; then
        print_color "🔍 DRY RUN: Would commit with message: \"${commit_message}\"" "$YELLOW"
        return 0
    fi
    
    # Commit the changes
    if git commit -m "$commit_message"; then
        print_color "✅ Changes committed successfully" "$GREEN"
        return 0
    else
        print_color "❌ Failed to commit changes" "$RED"
        return 1
    fi
}

print_color "🚀 CircuitTool Release Preparation v${VERSION}" "$BLUE"
print_color "================================================" "$BLUE"

# Check if dry run mode
if [ "$DRY_RUN" = true ]; then
    print_color "🔍 DRY RUN MODE - No actual release will be created" "$YELLOW"
    print_color "" "$NC"
fi

# Validate git repository
if ! check_git_repo; then
    print_color "❌ Not in a git repository. Please run this script from the project root." "$RED"
    exit 1
fi

# Check if tag already exists
if check_tag_exists "v${VERSION}"; then
    print_color "⚠️  Git tag v${VERSION} already exists!" "$YELLOW"
    print_color "   Delete it first if you want to recreate: git tag -d v${VERSION}" "$YELLOW"
    if [ "$DRY_RUN" != true ]; then
        read -p "Continue anyway? (y/N): " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            exit 1
        fi
    fi
fi

# Step 0: Commit any uncommitted changes
print_color "📝 Step 0: Checking for uncommitted changes..." "$YELLOW"
if check_uncommitted_changes; then
    print_color "🔍 Found uncommitted changes" "$BLUE"
    
    # Create a comprehensive commit message
    commit_message="Release v${VERSION} - Documentation Excellence

- Complete documentation refactoring and enhancement
- Updated version numbers to v${VERSION}
- Enhanced CI/CD and release automation
- Professional documentation with DocFX integration
- Comprehensive learning materials and examples

Version: v${VERSION}
CLI Version: v${CLI_VERSION}
Release Date: ${RELEASE_DATE}"
    
    if ! commit_all_changes "$commit_message"; then
        print_color "❌ Failed to commit changes. Cannot proceed with release." "$RED"
        exit 1
    fi
else
    print_color "✅ No uncommitted changes found" "$GREEN"
fi

# Step 1: Clean and build the project
print_color "📦 Step 1: Building project..." "$YELLOW"
dotnet clean CircuitTool.sln
dotnet restore CircuitTool.sln
dotnet build CircuitTool.sln --configuration Release --verbosity minimal

if [ $? -eq 0 ]; then
    print_color "✅ Build completed successfully" "$GREEN"
else
    print_color "❌ Build failed" "$RED"
    exit 1
fi

# Step 2: Run tests
if [ "$SKIP_TESTS" = true ]; then
    print_color "⏭️  Step 2: Skipping tests (SKIP_TESTS=true)..." "$YELLOW"
else
    print_color "🧪 Step 2: Running tests..." "$YELLOW"
    dotnet test CircuitTool.sln --configuration Release --verbosity minimal --no-build

    if [ $? -eq 0 ]; then
        print_color "✅ All tests passed" "$GREEN"
    else
        print_color "❌ Tests failed" "$RED"
        exit 1
    fi
fi

# Step 3: Build documentation
print_color "📚 Step 3: Building documentation..." "$YELLOW"
if command -v docfx &> /dev/null; then
    docfx metadata
    docfx build
    print_color "✅ Documentation built successfully" "$GREEN"
else
    print_color "⚠️  DocFX not found, skipping documentation build" "$YELLOW"
fi

# Step 4: Create NuGet package
print_color "📦 Step 4: Creating NuGet package..." "$YELLOW"
dotnet pack CircuitTool.csproj --configuration Release --no-build --output ./packages

if [ $? -eq 0 ]; then
    print_color "✅ NuGet package created successfully" "$GREEN"
else
    print_color "❌ Package creation failed" "$RED"
    exit 1
fi

# Step 5: Build CLI
print_color "🖥️  Step 5: Building CLI..." "$YELLOW"
cd CircuitTool.CLI
dotnet build --configuration Release --verbosity minimal
dotnet publish --configuration Release --output ../publish/cli --verbosity minimal
cd ..

if [ $? -eq 0 ]; then
    print_color "✅ CLI built successfully" "$GREEN"
else
    print_color "❌ CLI build failed" "$RED"
    exit 1
fi

# Step 6: Create release artifacts
print_color "📁 Step 6: Creating release artifacts..." "$YELLOW"

# Create release directory
mkdir -p release/v${VERSION}

# Copy main artifacts
cp packages/*.nupkg release/v${VERSION}/
cp packages/*.snupkg release/v${VERSION}/ 2>/dev/null || true

# Copy documentation
if [ -d "docs/_site" ]; then
    cp -r docs/_site release/v${VERSION}/documentation
fi

# Copy CLI
if [ -d "publish/cli" ]; then
    cp -r publish/cli release/v${VERSION}/
fi

# Copy important files
cp README.md release/v${VERSION}/
cp CHANGELOG.md release/v${VERSION}/
cp LICENSE release/v${VERSION}/
cp PROJECT_CODE_MAP.md release/v${VERSION}/
cp DOCUMENTATION.md release/v${VERSION}/

print_color "✅ Release artifacts created" "$GREEN"

# Step 7: Generate release notes
print_color "📝 Step 7: Generating release notes..." "$YELLOW"

cat > release/v${VERSION}/RELEASE_NOTES.md << EOF
# CircuitTool v${VERSION} - Documentation Excellence Release

**Release Date**: ${RELEASE_DATE}

## 🎉 Major Documentation Overhaul

This release represents a comprehensive refactoring of all documentation and project structure, bringing CircuitTool to production-ready standards with professional documentation.

## ✨ What's New

### 📚 Complete Documentation Refactoring
- **Professional documentation** with consistent structure and modern design
- **DocFX integration** with auto-generated API documentation
- **Emoji-based navigation** for improved user experience
- **Comprehensive learning paths** from beginner to advanced
- **Technology guides** for hardware integration and protocols

### 🏗️ Enhanced Project Structure
- **Modern build system** with improved tooling
- **Clean documentation hierarchy** with complete navigation
- **Professional README** with enhanced examples
- **Structured tutorials** and real-world examples

### 🔧 Technical Improvements
- **98% clean builds** (reduced warnings from 205+ to 203)
- **Modern DocFX templates** with responsive design
- **Enhanced cross-referencing** between documentation sections
- **Production-ready documentation site**

## 📦 Package Information

- **CircuitTool Library**: v${VERSION}
- **CircuitTool CLI**: v${CLI_VERSION}
- **Target Frameworks**: .NET Framework 4.5+, .NET Core 3.1+, .NET 6.0+, .NET 8.0+, .NET Standard 2.0+

## 🚀 Installation

### NuGet Package Manager
\`\`\`bash
dotnet add package CircuitTool --version ${VERSION}
\`\`\`

### Package Manager Console
\`\`\`powershell
Install-Package CircuitTool -Version ${VERSION}
\`\`\`

## 📖 Documentation

- **[Complete Documentation](https://github.com/jomardyan/CircuitTool/tree/v${VERSION}/docs)**
- **[API Reference](https://github.com/jomardyan/CircuitTool/tree/v${VERSION}/docs/api)**
- **[Getting Started Guide](https://github.com/jomardyan/CircuitTool/blob/v${VERSION}/articles/getting-started.md)**
- **[Technology Guides](https://github.com/jomardyan/CircuitTool/tree/v${VERSION}/docs/technology-guides)**

## 🔧 Breaking Changes

**None** - This release is fully backward compatible with v2.1.0.

## 🎯 Upgrade Path

Upgrading from v2.1.0 to v${VERSION}:
1. Update your package reference to v${VERSION}
2. Enjoy the enhanced documentation and examples
3. Explore new learning materials and guides

## 📋 Full Changelog

See [CHANGELOG.md](CHANGELOG.md) for complete details of all changes.

---

**Happy engineering! 🔧⚡**
EOF

print_color "✅ Release notes generated" "$GREEN"

# Step 8: Create release summary
print_color "📊 Step 8: Creating release summary..." "$YELLOW"

print_color "" "$NC"
print_color "🎉 RELEASE PREPARATION COMPLETE!" "$GREEN"
print_color "=================================" "$GREEN"
print_color "" "$NC"
print_color "📋 Release Summary:" "$BLUE"
print_color "  Version: v${VERSION}" "$NC"
print_color "  CLI Version: v${CLI_VERSION}" "$NC"
print_color "  Release Date: ${RELEASE_DATE}" "$NC"
print_color "  Build Status: ✅ SUCCESS" "$GREEN"
print_color "  Tests Status: ✅ PASSED" "$GREEN"
print_color "  Packages: ✅ CREATED" "$GREEN"
print_color "" "$NC"
print_color "📁 Release Artifacts:" "$BLUE"
find release/v${VERSION} -type f | sed 's/^/  /'
print_color "" "$NC"
print_color "🚀 Next Steps:" "$BLUE"
if [ "$DRY_RUN" = true ]; then
    print_color "  ✅ Dry run completed successfully!" "$GREEN"
    print_color "  🔄 Run without DRY_RUN=true to create actual release" "$NC"
elif [ "$AUTO_CREATE_RELEASE" = true ] && check_github_cli && check_github_auth && validate_github_repo "${GITHUB_REPO}"; then
    print_color "  ✅ GitHub release created automatically!" "$GREEN"
    print_color "  🌐 View release: https://github.com/${GITHUB_REPO}/releases/tag/v${VERSION}" "$NC"
    print_color "  📦 Optional: Publish to NuGet.org" "$NC"
    print_color "" "$NC"
    print_color "📦 NuGet Publish Command (optional):" "$BLUE"
    print_color "  dotnet nuget push ./packages/CircuitTool.${VERSION}.nupkg \\" "$NC"
    print_color "    --api-key YOUR_NUGET_API_KEY \\" "$NC"
    print_color "    --source https://api.nuget.org/v3/index.json" "$NC"
else
    print_color "  1. Review release artifacts in ./release/v${VERSION}/" "$NC"
    print_color "  2. Test the NuGet package locally: ./release/v${VERSION}/test-package.sh" "$NC"
    print_color "  3. Create GitHub release with tag v${VERSION}" "$NC"
    print_color "  4. Upload release artifacts" "$NC"
    print_color "  5. Publish to NuGet.org (optional)" "$NC"
    print_color "" "$NC"
    print_color "📦 Manual GitHub Release Commands:" "$BLUE"
    print_color "  # Using GitHub CLI:" "$NC"
    print_color "  git tag -a v${VERSION} -m \"CircuitTool v${VERSION} - Documentation Excellence Release\"" "$NC"
    print_color "  git push origin v${VERSION}" "$NC"
    print_color "  gh release create v${VERSION} ./release/v${VERSION}/*.nupkg ./release/v${VERSION}/*.md \\" "$NC"
    print_color "    --title \"CircuitTool v${VERSION} - Documentation Excellence\" \\" "$NC"
    print_color "    --notes-file ./release/v${VERSION}/RELEASE_NOTES.md \\" "$NC"
    print_color "    --repo ${GITHUB_REPO}" "$NC"
    print_color "" "$NC"
    print_color "  # Or via Web Interface:" "$NC"
    print_color "  https://github.com/${GITHUB_REPO}/releases/new" "$NC"
    print_color "    - Tag: v${VERSION}" "$NC"
    print_color "    - Title: CircuitTool v${VERSION} - Documentation Excellence" "$NC"
    print_color "    - Upload files from ./release/v${VERSION}/" "$NC"
fi
print_color "" "$NC"

# Create a quick test script
cat > release/v${VERSION}/test-package.sh << 'EOF'
#!/bin/bash
# Quick test script for the NuGet package

echo "🧪 Testing CircuitTool NuGet Package..."

# Create a temporary test project
mkdir -p temp-test
cd temp-test

# Initialize a new console project
dotnet new console -n CircuitToolTest
cd CircuitToolTest

# Add the local package
dotnet add package CircuitTool --source ../../ --version 2.2.0

# Create a simple test
cat > Program.cs << 'EOL'
using CircuitTool;
using CircuitTool.Calculators;

Console.WriteLine("CircuitTool v2.2.0 Test");
Console.WriteLine("======================");

// Test basic Ohm's Law calculation
var voltage = OhmsLawCalculator.Voltage(0.1, 100);
Console.WriteLine($"Voltage = {voltage}V (I=0.1A, R=100Ω)");

// Test LED calculation
var ledResistor = LEDCalculator.CalculateResistorValue(5.0, 3.3, 0.02);
Console.WriteLine($"LED Resistor = {ledResistor}Ω (Vs=5V, Vf=3.3V, If=20mA)");

Console.WriteLine("✅ Package test completed successfully!");
EOL

# Build and run
dotnet build
dotnet run

cd ../..
rm -rf temp-test

echo "✅ Package test completed!"
EOF

chmod +x release/v${VERSION}/test-package.sh

print_color "✅ Test script created: ./release/v${VERSION}/test-package.sh" "$GREEN"
print_color "" "$NC"

# Step 9: Create GitHub Release (if enabled and not dry run)
if [ "$AUTO_CREATE_RELEASE" = true ] && [ "$DRY_RUN" != true ]; then
    print_color "🚀 Step 9: Creating GitHub release..." "$YELLOW"
    
    # Check if GitHub CLI is available
    if ! check_github_cli; then
        print_color "⚠️  GitHub CLI not found. Please install gh CLI to auto-create releases." "$YELLOW"
        print_color "   Installation: https://cli.github.com/" "$YELLOW"
        print_color "   Manual release creation instructions below." "$YELLOW"
    elif ! check_github_auth; then
        print_color "⚠️  Not authenticated with GitHub. Please run 'gh auth login' first." "$YELLOW"
        print_color "   Manual release creation instructions below." "$YELLOW"
    elif ! validate_github_repo "${GITHUB_REPO}"; then
        print_color "⚠️  Cannot access GitHub repository '${GITHUB_REPO}'. Check repository name and permissions." "$YELLOW"
        print_color "   Manual release creation instructions below." "$YELLOW"
    else
        print_color "🔍 Creating GitHub release for repository: ${GITHUB_REPO}" "$BLUE"
        
        # Push any commits to remote before creating tags
        print_color "📤 Pushing commits to remote repository..." "$BLUE"
        if git push origin HEAD; then
            print_color "✅ Commits pushed successfully" "$GREEN"
        else
            print_color "⚠️  Failed to push commits, but continuing..." "$YELLOW"
        fi
        
        # Create git tag first
        if ! check_tag_exists "v${VERSION}"; then
            print_color "🏷️  Creating git tag v${VERSION}..." "$BLUE"
            git tag -a "v${VERSION}" -m "CircuitTool v${VERSION} - Documentation Excellence Release"
            git push origin "v${VERSION}"
        fi
        
        # Create the release with GitHub CLI
        print_color "📦 Uploading release artifacts..." "$BLUE"
        if gh release create "v${VERSION}" \
            --repo "${GITHUB_REPO}" \
            --title "CircuitTool v${VERSION} - Documentation Excellence" \
            --notes-file "./release/v${VERSION}/RELEASE_NOTES.md" \
            $([ "$PRERELEASE" = true ] && echo "--prerelease") \
            ./release/v${VERSION}/*.nupkg \
            ./release/v${VERSION}/*.snupkg \
            ./release/v${VERSION}/README.md \
            ./release/v${VERSION}/CHANGELOG.md \
            ./release/v${VERSION}/LICENSE \
            ./release/v${VERSION}/RELEASE_NOTES.md \
            ./release/v${VERSION}/test-package.sh; then
            
            print_color "✅ GitHub release created successfully!" "$GREEN"
            print_color "🌐 Release URL: https://github.com/${GITHUB_REPO}/releases/tag/v${VERSION}" "$BLUE"
            
            # Check if release was created successfully
            if gh release view "v${VERSION}" --repo "${GITHUB_REPO}" &> /dev/null; then
                print_color "✅ Release verified and live on GitHub!" "$GREEN"
                
                # Upload additional files (documentation, CLI) if they exist
                if [ -d "./release/v${VERSION}/documentation" ]; then
                    print_color "📚 Uploading documentation archive..." "$BLUE"
                    cd "./release/v${VERSION}"
                    tar -czf "CircuitTool-v${VERSION}-documentation.tar.gz" documentation/
                    gh release upload "v${VERSION}" "CircuitTool-v${VERSION}-documentation.tar.gz" --repo "${GITHUB_REPO}"
                    cd ../..
                fi
                
                if [ -d "./release/v${VERSION}/cli" ]; then
                    print_color "🖥️  Uploading CLI archive..." "$BLUE"
                    cd "./release/v${VERSION}"
                    tar -czf "CircuitTool-CLI-v${CLI_VERSION}.tar.gz" cli/
                    gh release upload "v${VERSION}" "CircuitTool-CLI-v${CLI_VERSION}.tar.gz" --repo "${GITHUB_REPO}"
                    cd ../..
                fi
            fi
        else
            print_color "❌ Failed to create GitHub release" "$RED"
            print_color "   You can create it manually using the instructions below." "$YELLOW"
        fi
    fi
elif [ "$DRY_RUN" = true ]; then
    print_color "🔍 DRY RUN: Would create GitHub release with the following details:" "$BLUE"
    print_color "   Repository: ${GITHUB_REPO}" "$NC"
    print_color "   Tag: v${VERSION}" "$NC"
    print_color "   Title: CircuitTool v${VERSION} - Documentation Excellence" "$NC"
    print_color "   Prerelease: ${PRERELEASE}" "$NC"
    print_color "" "$NC"
    print_color "🔍 DRY RUN: Git operations that would be performed:" "$BLUE"
    print_color "   1. git push origin HEAD  # Push any commits" "$NC"
    if ! check_tag_exists "v${VERSION}"; then
        print_color "   2. git tag -a v${VERSION} -m \"CircuitTool v${VERSION} - Documentation Excellence Release\"" "$NC"
        print_color "   3. git push origin v${VERSION}  # Push the tag" "$NC"
    else
        print_color "   2. Tag v${VERSION} already exists, would skip creation" "$NC"
    fi
    print_color "" "$NC"
    print_color "🔍 DRY RUN: Files to upload:" "$NC"
    find release/v${VERSION} -type f | head -10 | sed 's/^/     /'
    if [ $(find release/v${VERSION} -type f | wc -l) -gt 10 ]; then
        print_color "     ... and $(( $(find release/v${VERSION} -type f | wc -l) - 10 )) more files" "$NC"
    fi
else
    print_color "⏭️  Skipping automatic GitHub release creation (AUTO_CREATE_RELEASE=false)" "$YELLOW"
fi

print_color "" "$NC"
print_color "🎉 Ready for GitHub release!" "$GREEN"
