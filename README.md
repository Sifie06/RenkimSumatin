# Renkim Sumatin

WPF .NET 8 application with WiX Toolset 4 MSI packaging. This repo includes a GitHub Actions workflow to build the app, create the MSI, and publish artifacts.

## Build locally
- .NET 8 SDK
- WiX Toolset 4 SDK (via NuGet, handled by `dotnet restore`)

Commands:
- Build app: `dotnet build ./renkimsumatin.csproj -c Release`
- Build installer: `dotnet build ./Installer/Installer.wixproj -c Release`
- Publish app: `dotnet publish ./renkimsumatin.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=false -o ./publish/win-x64`

## CI
See `.github/workflows/build.yml`. Push to `main` or create a tag like `v1.0.0` to produce a release with the MSI attached.
