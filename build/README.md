# 🏗️ Build & Deployment Scripts

This folder contains all the build, deployment, and development scripts for CircuitTool.

## 📂 Scripts Overview

### 🚀 Release & Publishing
- **`prepare-release.sh`** - Main automated release script with version auto-increment
- **`publish.sh`** / **`publish.ps1`** - NuGet package publishing scripts

### 🖥️ CLI Tools
- **`run-cli.sh`** / **`run-cli.bat`** - Quick CLI launcher scripts

### 📚 Documentation
- **`generate-docs.sh`** / **`generate-docs.ps1`** - DocFX documentation generation
- **`docs.bat`** - Windows documentation batch script

## 🚀 Quick Start

### Automated Release (Recommended)
```bash
# Run from project root
./build/prepare-release.sh

# With options
VERSION_INCREMENT=minor ./build/prepare-release.sh
DRY_RUN=true ./build/prepare-release.sh
```

### Generate Documentation
```bash
# Run from project root
./build/generate-docs.sh

# Or on Windows
./build/generate-docs.ps1
```

### Run CLI Tool
```bash
./build/run-cli.sh
```

## 🔧 Environment Setup

All scripts expect to be run from the **project root directory** and will automatically navigate to the correct locations.

### Prerequisites
- .NET SDK 6.0 or later
- Git with proper repository setup
- GitHub CLI (for automated releases)
- DocFX (for documentation generation)

## 📝 Script Details

### prepare-release.sh
- **Auto-increments** version numbers (patch/minor/major)
- **Updates** all project files with new versions
- **Builds** and tests the entire solution
- **Generates** documentation and packages
- **Creates** GitHub releases automatically
- **Commits** all changes with proper messages

### Configuration Files
The `config/` folder contains:
- `docfx.json` - DocFX documentation configuration
- `nuget.config` - NuGet package source configuration

## 🎯 Usage Examples

```bash
# Full automated release (patch increment)
./build/prepare-release.sh

# Minor version release
VERSION_INCREMENT=minor ./build/prepare-release.sh

# Preview what would happen (dry run)
DRY_RUN=true ./build/prepare-release.sh

# Build without GitHub release
AUTO_CREATE_RELEASE=false ./build/prepare-release.sh

# Skip tests for faster iteration
SKIP_TESTS=true ./build/prepare-release.sh

# Create pre-release
PRERELEASE=true ./build/prepare-release.sh
```

## 🔍 Troubleshooting

If you encounter issues:

1. **Ensure you're in the project root directory**
2. **Check Git repository status** (`git status`)
3. **Verify GitHub CLI authentication** (`gh auth status`)
4. **Run dry run first** (`DRY_RUN=true ./build/prepare-release.sh`)

## 📁 Related Folders

- **`assets/`** - Images and icons
- **`config/`** - Configuration files
- **`docs/`** - Documentation source files
- **`src/`** - Source code
- **`tests/`** - Unit tests
