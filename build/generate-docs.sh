#!/bin/bash

# Documentation generation script for CircuitTool
# This script installs DocFX and generates HTML documentation

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Default values
OUTPUT_PATH="_site"
INSTALL_ONLY=false
BUILD_ONLY=false
SERVE_ONLY=false
CLEAN_ONLY=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        --install)
            INSTALL_ONLY=true
            shift
            ;;
        --build)
            BUILD_ONLY=true
            shift
            ;;
        --serve)
            SERVE_ONLY=true
            shift
            ;;
        --clean)
            CLEAN_ONLY=true
            shift
            ;;
        --output)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        --help)
            echo "Usage: $0 [--install|--build|--serve|--clean] [--output OUTPUT_PATH]"
            echo "  --install    Install DocFX only"
            echo "  --build      Build documentation only"
            echo "  --serve      Serve documentation only"
            echo "  --clean      Clean generated files only"
            echo "  --output     Specify output directory (default: _site)"
            echo "  --help       Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

echo -e "${CYAN}CircuitTool Documentation Generator${NC}"
echo -e "${CYAN}===================================${NC}"

# Check if DocFX is installed
check_docfx_installed() {
    if command -v docfx &> /dev/null; then
        local version=$(docfx --version 2>/dev/null || echo "unknown")
        echo -e "${GREEN}✓ DocFX is already installed: $version${NC}"
        return 0
    else
        echo -e "${YELLOW}✗ DocFX not found${NC}"
        return 1
    fi
}

# Install DocFX
install_docfx() {
    echo -e "${YELLOW}Installing DocFX...${NC}"
    
    if command -v dotnet &> /dev/null; then
        echo -e "${BLUE}Installing DocFX as a global .NET tool...${NC}"
        dotnet tool install -g docfx
        
        if [ $? -eq 0 ]; then
            echo -e "${GREEN}✓ DocFX installed successfully!${NC}"
        else
            echo -e "${RED}✗ Failed to install DocFX via dotnet tool${NC}"
            
            # Fallback: Try to download DocFX manually
            echo -e "${YELLOW}Trying alternative installation method...${NC}"
            
            DOCFX_URL="https://github.com/dotnet/docfx/releases/download/v2.75.3/docfx-linux-x64-v2.75.3.zip"
            DOCFX_ZIP="docfx.zip"
            DOCFX_DIR="docfx"
            
            if command -v wget &> /dev/null; then
                wget -O "$DOCFX_ZIP" "$DOCFX_URL"
            elif command -v curl &> /dev/null; then
                curl -L -o "$DOCFX_ZIP" "$DOCFX_URL"
            else
                echo -e "${RED}✗ Neither wget nor curl found. Cannot download DocFX.${NC}"
                exit 1
            fi
            
            unzip -o "$DOCFX_ZIP" -d "$DOCFX_DIR"
            rm "$DOCFX_ZIP"
            
            # Make executable and add to PATH temporarily
            chmod +x "$DOCFX_DIR/docfx"
            export PATH="$PWD/$DOCFX_DIR:$PATH"
            
            echo -e "${GREEN}✓ DocFX downloaded and extracted!${NC}"
        fi
    else
        echo -e "${RED}✗ .NET CLI not found. Please install .NET SDK first.${NC}"
        exit 1
    fi
}

# Build documentation
build_documentation() {
    echo -e "${YELLOW}Building documentation...${NC}"
    
    if [ ! -f "config/docfx.json" ]; then
        echo -e "${RED}✗ config/docfx.json not found. Make sure you're in the project root directory.${NC}"
        exit 1
    fi
    
    # Build the project first to generate XML documentation
    echo -e "${BLUE}Building CircuitTool project...${NC}"
    dotnet build --configuration Release
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}✗ Project build failed${NC}"
        exit 1
    fi
    
    # Generate API documentation
    echo -e "${BLUE}Generating API documentation...${NC}"
    docfx metadata config/docfx.json
    
    if [ $? -ne 0 ]; then
        echo -e "${RED}✗ API documentation generation failed${NC}"
        exit 1
    fi
    
    # Build HTML documentation
    echo -e "${BLUE}Building HTML documentation...${NC}"
    docfx build config/docfx.json
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓ Documentation built successfully!${NC}"
        echo -e "${CYAN}Documentation available at: $OUTPUT_PATH/index.html${NC}"
    else
        echo -e "${RED}✗ Documentation build failed${NC}"
        exit 1
    fi
}

# Serve documentation locally
serve_documentation() {
    echo -e "${YELLOW}Starting documentation server...${NC}"
    
    if [ ! -f "$OUTPUT_PATH/index.html" ]; then
        echo -e "${YELLOW}Documentation not found. Building first...${NC}"
        build_documentation
    fi
    
    echo -e "${CYAN}Documentation server starting at http://localhost:8080${NC}"
    echo -e "${NC}Press Ctrl+C to stop the server${NC}"
    
    docfx serve "$OUTPUT_PATH" --port 8080
}

# Clean generated files
clean_documentation() {
    echo -e "${YELLOW}Cleaning generated documentation files...${NC}"
    
    local paths_to_clean=("_site" "api" "obj")
    
    for path in "${paths_to_clean[@]}"; do
        if [ -d "$path" ]; then
            rm -rf "$path"
            echo -e "${GREEN}✓ Cleaned $path${NC}"
        fi
    done
    
    echo -e "${GREEN}✓ Cleanup completed!${NC}"
}

# Main script logic
if [ "$INSTALL_ONLY" = true ]; then
    if ! check_docfx_installed; then
        install_docfx
    fi
elif [ "$BUILD_ONLY" = true ]; then
    if ! check_docfx_installed; then
        echo -e "${YELLOW}DocFX not installed. Installing first...${NC}"
        install_docfx
    fi
    build_documentation
elif [ "$SERVE_ONLY" = true ]; then
    if ! check_docfx_installed; then
        echo -e "${YELLOW}DocFX not installed. Installing first...${NC}"
        install_docfx
    fi
    serve_documentation
elif [ "$CLEAN_ONLY" = true ]; then
    clean_documentation
else
    # Default: Install, build, and optionally serve
    if ! check_docfx_installed; then
        install_docfx
    fi
    build_documentation
    
    echo ""
    echo -e "${CYAN}Would you like to start the documentation server? (y/n)${NC}"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        serve_documentation
    else
        echo -e "${GREEN}Documentation is ready at: $OUTPUT_PATH/index.html${NC}"
    fi
fi

echo ""
echo -e "${GREEN}Documentation generation completed!${NC}"
