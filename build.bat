@echo off
REM Gamer Optimizer - Build and Deploy Script

SETLOCAL ENABLEDELAYEDEXPANSION

echo.
echo ===============================================
echo  Gamer Optimizer - Build Script
echo ===============================================
echo.

REM Resolve MSBuild path (PATH, common install dirs, or vswhere)
set "MSBUILD_EXE="
for /f "delims=" %%i in ('where msbuild 2^>nul') do set "MSBUILD_EXE=%%i"

if not defined MSBUILD_EXE (
    if exist "C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
if not defined MSBUILD_EXE (
    if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe"
)
if not defined MSBUILD_EXE (
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe" set "MSBUILD_EXE=C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
)
if not defined MSBUILD_EXE (
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" (
        for /f "usebackq tokens=*" %%i in (`"C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe`) do set "MSBUILD_EXE=%%i"
    )
)

if not defined MSBUILD_EXE (
    echo [ERROR] MSBuild not found. Please install Visual Studio Build Tools 2022.
    echo        Workload: Managed Desktop Build Tools, plus .NET 4.8 SDK/Targeting Pack.
    pause
    exit /b 1
)

REM Check if running in project root
if not exist "Optimizer.sln" (
    echo [ERROR] Optimizer.sln not found. Please run this script from the project root.
    pause
    exit /b 1
)

REM Ensure NuGet CLI is available (download locally if missing)
set "NUGET_CMD=nuget"
where nuget >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    if not exist "%cd%\nuget.exe" (
        echo [INFO] Downloading NuGet CLI...
        powershell -NoProfile -ExecutionPolicy Bypass -Command "try { Invoke-WebRequest -UseBasicParsing -Uri https://dist.nuget.org/win-x86-commandline/latest/nuget.exe -OutFile nuget.exe } catch { Write-Error $_; exit 1 }"
        if %ERRORLEVEL% NEQ 0 (
            echo [ERROR] Failed to download nuget.exe
            pause
            exit /b 1
        )
    )
    set "NUGET_CMD=%cd%\nuget.exe"
)

echo [1/6] Cleaning previous build...
"%MSBUILD_EXE%" Optimizer.sln /p:Configuration=Release /t:Clean >nul 2>nul

echo [2/6] Restoring NuGet packages...
"%NUGET_CMD%" restore Optimizer.sln >nul 2>nul

echo [3/6] Building solution...
"%MSBUILD_EXE%" Optimizer.sln /p:Configuration=Release /p:Platform=AnyCPU
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Build failed!
    pause
    exit /b 1
)

echo [4/6] Verifying build output...
if not exist "Optimizer\bin\Release\Optimizer.exe" (
    echo [ERROR] Build output not found!
    pause
    exit /b 1
)

echo [5/6] Build completed successfully!
echo.
echo ===============================================
echo  Build Output:
echo ===============================================
echo Output file: %cd%\Optimizer\bin\Release\Optimizer.exe
echo.
echo Next steps:
echo 1. Run the application (as Administrator)
echo 2. Ensure backend API is running on VPS
echo 3. Enter test license key: GAMOPT-2026-TEST-001
echo.
echo ===============================================
echo.

REM Ask to run the application
set /p run="Would you like to run the application now? (y/n): "
if /i "%run%"=="y" (
    echo Running GamerOptimizer as Administrator...
    cd Optimizer\bin\Release
    powershell -Command "Start-Process Optimizer.exe -Verb RunAs"
    cd ..\..\..
)

echo [6/6] Done!
pause
