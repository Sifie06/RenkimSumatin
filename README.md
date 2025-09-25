# Renkim Sumatin

WPF .NET 8 application with WiX Toolset 4 MSI packaging. This repo includes a comprehensive GitHub Actions workflow to build the app, create the MSI installer, and publish downloadable artifacts.

## Features

- ✅ WPF .NET 8 application
- ✅ WiX Toolset 4 MSI installer with desktop shortcut
- ✅ Automated GitHub Actions CI/CD pipeline
- ✅ Downloadable MSI artifacts
- ✅ Automated releases with tags

## Downloading the MSI Installer

### Option 1: Download from GitHub Actions (Recommended)

1. **Navigate to Actions**: Go to the [Actions tab](https://github.com/Sifie06/RenkimSumatin/actions) of this repository
2. **Find Latest Build**: Click on the most recent successful "Build and Package" workflow run
3. **Download Artifact**: Scroll down to the "Artifacts" section and download `RenkimSumatin-Installer-v[X]` (where X is the build number)
4. **Extract and Install**: Extract the ZIP file and run the `.msi` file to install the application

### Option 2: Download from Releases (For Tagged Versions)

1. **Navigate to Releases**: Go to the [Releases page](https://github.com/Sifie06/RenkimSumatin/releases)
2. **Download MSI**: Download the `.msi` file from the latest release
3. **Install**: Run the MSI file to install the application

### Option 3: Create a Release with Tag

To create a new release with the MSI:

```bash
git tag v1.0.0
git push origin v1.0.0
```

This will automatically trigger the build and create a GitHub release with the MSI attached.

## Build Locally

### Prerequisites
- .NET 8 SDK
- WiX Toolset 4 SDK (automatically downloaded via NuGet)

### Commands
```bash
# Restore dependencies
dotnet restore ./renkimsumatin.csproj
dotnet restore ./Installer/Installer.wixproj

# Build application
dotnet build ./renkimsumatin.csproj -c Release

# Publish application (required before building installer)
dotnet publish ./renkimsumatin.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=false /p:PublishTrimmed=false -o ./Installer/AppPublish

# Build MSI installer
dotnet build ./Installer/Installer.wixproj -c Release
```

The MSI file will be created in `./Installer/bin/x64/Release/`

## GitHub Actions Workflow

The automated build process includes:

1. **Environment Setup**: Windows latest with .NET 8
2. **Dependencies**: Restore NuGet packages for both app and installer
3. **Publishing**: Create self-contained win-x64 deployment
4. **MSI Creation**: Build installer using WiX Toolset 4
5. **Artifact Upload**: Make MSI downloadable from Actions tab
6. **Release Creation**: Automatic releases when tags are pushed

### Workflow Triggers

- ✅ Push to `main` branch
- ✅ Pull requests to `main` branch  
- ✅ Tags starting with `v*` (creates releases)
- ✅ Manual workflow dispatch

### Artifacts Created

- **RenkimSumatin-Installer-v[X]**: Contains the MSI installer file (retained for 90 days)
- **AppPublish-Contents-v[X]**: Contains published application files for debugging (retained for 30 days)

## Installation Features

The MSI installer provides:

- ✅ Installation to Program Files
- ✅ Desktop shortcut creation
- ✅ Proper uninstall support
- ✅ Windows registry integration
- ✅ Upgrade capability

## Project Structure

```
renkimsumatin.csproj          # Main WPF application
Installer/
├── Installer.wixproj         # WiX installer project
├── ProductManual.wxs         # WiX installer definition
.github/
└── workflows/
    └── build.yml             # GitHub Actions workflow
```
