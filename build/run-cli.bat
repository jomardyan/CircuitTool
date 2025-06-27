@echo off
REM CircuitTool CLI Build and Run Script

echo 🔧 Building CircuitTool CLI...

REM Navigate to the CLI directory
cd /d "%~dp0CircuitTool.CLI"

REM Restore NuGet packages
echo 📦 Restoring packages...
dotnet restore

REM Build the project
echo 🏗️  Building project...
dotnet build --configuration Release

if %ERRORLEVEL% EQU 0 (
    echo ✅ Build successful!
    echo.
    echo 🚀 Starting CircuitTool CLI...
    echo.
    
    REM Run the CLI application
    dotnet run --configuration Release -- %*
) else (
    echo ❌ Build failed!
    exit /b 1
)
