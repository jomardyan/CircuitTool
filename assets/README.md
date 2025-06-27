# 🎨 Project Assets

This folder contains visual assets and resources for the CircuitTool project.

## 📂 Files

### 🔷 `icon.png`
Project icon and logo:
- Used in NuGet packages
- GitHub repository icon
- Documentation branding
- Application icon for CLI tools

## 🎯 Usage

Assets are automatically included in:
- **NuGet packages** (icon.png as package icon)
- **Documentation** (logo in DocFX generated sites)
- **README** (project branding)
- **Release artifacts** (visual identification)

## 📏 Asset Guidelines

### Icon Requirements
- **Format**: PNG with transparency
- **Size**: 128x128 pixels (minimum)
- **Style**: Modern, clean, professional
- **Theme**: Electronics/circuit related

### Adding New Assets
1. Place files in this folder
2. Update relevant configuration files:
   - `CircuitTool.csproj` for package icon
   - `config/docfx.json` for documentation logo
   - README.md for repository branding

## 🔗 Related Files

- **`CircuitTool.csproj`** - References package icon
- **`config/docfx.json`** - Documentation logo configuration
- **`README.md`** - Project branding and badges
