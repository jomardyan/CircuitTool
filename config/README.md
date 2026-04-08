# ⚙️ Configuration Files

This folder contains configuration files for various tools and build processes.

## 📂 Files

### 📚 `docfx.json`
DocFX configuration for generating documentation:
- API documentation settings
- Template configuration
- Build and metadata options
- Site structure and navigation

### 📦 `nuget.config`
NuGet package source configuration:
- Package source URLs
- Authentication settings
- Package download configurations

## 🔧 Usage

These configuration files are automatically used by the build scripts:

- **Documentation generation**: `./build/generate-docs.sh` uses `docfx.json`
- **Package building**: Build scripts reference `nuget.config` for package sources
- **Release process**: `./build/prepare-release.sh` coordinates all configurations

## 🛠️ Customization

To customize the build process:

1. **Edit `docfx.json`** to modify documentation generation
2. **Edit `nuget.config`** to add custom package sources
3. Run `./build/generate-docs.sh` to regenerate documentation
4. Run `./build/prepare-release.sh` to test the full pipeline

## 📁 Related Folders

- **`build/`** - Build and deployment scripts
- **`docs/`** - Documentation source files
- **`assets/`** - Images and resources
