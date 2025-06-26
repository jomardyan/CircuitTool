# GitHub Actions Setup Guide

This repository uses GitHub Actions to automatically build, test, and publish the CircuitTool NuGet package. Here's how to set up and use the workflows.

## Project Overview

CircuitTool is a comprehensive C# library for electrical engineering and electronics calculations with support for:

### Core Calculator Classes
- `OhmsLawCalculator` - Basic Ohm's law calculations
- `ResistorCalculator` - Resistor network analysis
- `LEDCalculator` - LED circuit calculations
- `PowerCalculator` - Power analysis
- `VoltageCalculator` - Voltage calculations
- `VoltageDropCalculator` - Voltage drop analysis
- `VoltageDividerCalculator` - Voltage divider calculations
- `WattsVoltsAmpsOhmsCalculator` - Power relationship calculations

### Advanced Physics Calculators
- `CapacitorCalculator` - Capacitive circuits, reactance, energy storage, time constants
- `InductorCalculator` - Inductive circuits, reactance, energy storage, resonance
- `TransformerCalculator` - Transformer analysis, efficiency, regulation
- `ACCircuitCalculator` - AC circuit analysis, impedance, phase relationships
- `FilterCalculator` - Filter design and analysis (RC/RL low-pass/high-pass)

### Specialized Tools
- `ArduinoTools` - Arduino-specific calculations and conversions
- `ESP32Tools` - ESP32-specific calculations and power management
- `BeginnerCalculators` - Simplified calculators for common tasks
- `EnergyCalculator` - Energy consumption and cost calculations
- `PowerFactorCalculator` - Power factor and reactive power analysis
- `UnitConverter` - Electrical unit conversions
- `CircuitCalculations` - General circuit analysis methods

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
  - XML documentation generation for all target frameworks
  - Debug symbols (PDB files) included in packages
  - Package icon included
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
   git tag v1.0.9
   git push origin v1.0.9
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

### Legacy Publishing Scripts

For maintainers who prefer command-line publishing, legacy scripts are available:

**PowerShell (Windows):**
```powershell
# Publish to GitHub Packages
.\publish.ps1 -GitHub

# Publish to NuGet.org
.\publish.ps1 -NuGet

# Publish to both
.\publish.ps1 -All
```

**Bash (Linux/macOS):**
```bash
# Publish to GitHub Packages
./publish.sh --github

# Publish to NuGet.org
./publish.sh --nuget

# Publish to both
./publish.sh --all
```

**Note**: The automated GitHub Actions workflow is the recommended approach for publishing.

## Development and Maintenance

### For Developers

If you're contributing to CircuitTool development:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/jomardyan/CircuitTool.git
   cd CircuitTool
   ```

2. **Build the library**:
   ```bash
   dotnet build
   ```

3. **Run tests**:
   ```bash
   dotnet test
   ```

4. **Build packages locally**:
   ```bash
   dotnet pack --configuration Release --output ./packages
   ```

### For Maintainers

When maintaining the CircuitTool library:

1. **Update version**: Modify the `<Version>` in `CircuitTool.csproj`
2. **Update changelog**: Add release notes to `README.md`
3. **Commit changes**: Commit version and changelog updates
4. **Create release**: Use GitHub Actions workflows for publishing
5. **Verify packages**: Check both NuGet.org and GitHub Packages for successful publication

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

The project uses the version specified in `CircuitTool.csproj`. Current version is 1.0.12 which includes:

- **New Advanced Calculators**: Added comprehensive physics calculators (Capacitor, Inductor, Transformer, AC Circuit, Filter)
- **Test Framework Standardization**: Migrated all unit tests from MSTest to NUnit.Framework
- **Complete Test Coverage**: Added comprehensive unit tests for all calculators
- **Enhanced Documentation**: Updated with detailed usage examples for all calculators

```xml
<Version>1.0.12</Version>
```

To release a new version:
1. Update the version in `CircuitTool.csproj`
2. Commit the change
3. Create a tag matching the version (e.g., `v1.0.13`)
4. Create a GitHub release

## Testing Framework

The project uses **NUnit** as the testing framework. All tests are located in the `tests/` directory:

- **Framework**: NUnit.Framework only (MSTest has been removed)
- **Test Coverage**: All public methods in all calculator classes have corresponding unit tests
- **Test Execution**: Tests run automatically on all pull requests and pushes to main/develop branches
- **Test Files**: Each calculator class has its own dedicated test file (e.g., `CapacitorCalculatorTests.cs`)

### Running Tests Locally

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for a specific test file
dotnet test --filter "FullyQualifiedName~CapacitorCalculatorTests"
```

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
