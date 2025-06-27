@echo off
REM Batch file to generate CircuitTool documentation on Windows

echo CircuitTool Documentation Generator
echo ====================================
echo.

REM Check if PowerShell is available
where powershell >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: PowerShell is not available or not in PATH
    echo Please install PowerShell or use the manual method
    pause
    exit /b 1
)

REM Run the PowerShell script with appropriate parameters
if "%1"=="clean" (
    echo Running with clean option...
    powershell -ExecutionPolicy Bypass -File "generate-docs.ps1" -Clean -Build
) else if "%1"=="serve" (
    echo Running with serve option...
    powershell -ExecutionPolicy Bypass -File "generate-docs.ps1" -Build -Serve
) else if "%1"=="open" (
    echo Running with open option...
    powershell -ExecutionPolicy Bypass -File "generate-docs.ps1" -Build -Open
) else (
    echo Running standard documentation generation...
    powershell -ExecutionPolicy Bypass -File "generate-docs.ps1" -Build
)

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Documentation generation failed
    echo Check the output above for details
    pause
    exit /b 1
)

echo.
echo SUCCESS: Documentation generated successfully!
echo.
echo Available commands:
echo   docs.bat         - Generate documentation
echo   docs.bat clean   - Clean and regenerate documentation
echo   docs.bat serve   - Generate and serve documentation locally
echo   docs.bat open    - Generate and open documentation in browser
echo.
pause
