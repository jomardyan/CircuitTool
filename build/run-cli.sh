#!/bin/bash

# CircuitTool CLI Build and Run Script

echo "🔧 Building CircuitTool CLI..."

# Navigate to the CLI directory
cd "$(dirname "$0")/CircuitTool.CLI"

# Restore NuGet packages
echo "📦 Restoring packages..."
dotnet restore

# Build the project
echo "🏗️  Building project..."
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo ""
    echo "🚀 Starting CircuitTool CLI..."
    echo ""
    
    # Run the CLI application
    dotnet run --configuration Release -- "$@"
else
    echo "❌ Build failed!"
    exit 1
fi
