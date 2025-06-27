# 📁 Project Structure

The CircuitTool project has been organized into logical folders for better maintainability and navigation.

## 🏗️ Folder Organization

```
CircuitTool/
├── 📦 build/           # Build, deployment, and development scripts
├── ⚙️ config/          # Configuration files (DocFX, NuGet, etc.)
├── 🎨 assets/          # Visual assets (icons, images)
├── 📚 docs/            # Documentation source files
├── 💾 src/             # Library source code
├── 🧪 tests/           # Unit tests
├── 🖥️ CircuitTool.CLI/ # Command-line interface project
├── 📁 packages/        # Generated NuGet packages
├── 🚀 publish/         # CLI publishing output
├── 📋 release/         # Release artifacts
├── 🔧 bin/             # Build output
└── 📂 obj/             # Build intermediates
```

## 🎯 Quick Navigation

### 🚀 **Getting Started**
- **Main Documentation**: [`docs/`](docs/)
- **Quick Examples**: [`docs/examples/`](docs/examples/)
- **API Reference**: [`docs/api/`](docs/api/)

### 🔧 **Development**
- **Source Code**: [`src/`](src/)
- **Tests**: [`tests/`](tests/)
- **Build Scripts**: [`build/`](build/)

### 📦 **Release & Distribution**
- **Automated Release**: `./build/prepare-release.sh`
- **Manual CLI**: `./build/run-cli.sh`
- **Documentation**: `./build/generate-docs.sh`

## 🛠️ Common Tasks

### 🏃‍♂️ Quick Start
```bash
# Run the CLI tool
./build/run-cli.sh

# Generate documentation
./build/generate-docs.sh

# Create a release
./build/prepare-release.sh
```

### 🔄 Development Workflow
```bash
# Build and test
dotnet build
dotnet test

# Generate docs locally
./build/generate-docs.sh

# Preview release (dry run)
DRY_RUN=true ./build/prepare-release.sh
```

### 📋 File Locations Reference

| **File Type** | **Location** | **Purpose** |
|---------------|--------------|-------------|
| **Build Scripts** | `build/` | Automation and deployment |
| **Configuration** | `config/` | Tool configurations |
| **Assets** | `assets/` | Icons and images |
| **Documentation** | `docs/` | User guides and API docs |
| **Source Code** | `src/` | Library implementation |
| **Tests** | `tests/` | Unit and integration tests |
| **CLI Tool** | `CircuitTool.CLI/` | Command-line interface |

## 📖 Detailed Documentation

Each folder contains its own README with specific details:
- [`build/README.md`](build/README.md) - Build and deployment scripts
- [`config/README.md`](config/README.md) - Configuration files
- [`assets/README.md`](assets/README.md) - Project assets
- [`docs/README.md`](docs/README.md) - Documentation guide

## 🎉 Benefits of This Structure

✅ **Clear separation** of concerns  
✅ **Easy navigation** for new contributors  
✅ **Consistent organization** across the project  
✅ **Scalable structure** for future growth  
✅ **Professional presentation** for open source

---

*This structure makes CircuitTool more maintainable and contributor-friendly while keeping everything organized and accessible.*
