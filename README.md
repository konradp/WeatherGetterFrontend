# WeatherGetterFrontend

A modern weather information web application built with ASP.NET Core 8.0 and Blazor Server components. This application communicates directly with backend app [WeatherGetter](https://github.com/konradp/WeatherGetter)

## Features

- View current weather data for various locations
- Browse and manage different cities
- Compare weather conditions between multiple locations
- Visualize weather data using Chart.js
- Bootstrap-powered responsive UI
- Automated tests with xUnit, bUnit, and Playwright

## Technologies

- ASP.NET Core 8.0
- Blazor Server Components
- Bootstrap
- Chart.js v3.7.1
- C# with .NET 8.0
- CSS with Bootstrap

## Project Structure

- `WeatherGetterFrontend.Tests` - unit/component tests (xUnit + bUnit)
- `WeatherGetterFrontend.E2E` - end-to-end smoke tests (xUnit + Playwright)

## Getting Started

### Configuration

The application uses configuration settings from:
- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development settings

### API Configuration

Update the `ApiBaseUrl` in your configuration files to point to your weather API backend:

```json
{
  "ApiBaseUrl": "https://your-weather-api-url.com/"
}
```

### Build & Run

```bash
# Development mode
dotnet run

# Production build
dotnet publish -c Release
```

### Testing

```bash
# Component and unit tests
dotnet test WeatherGetterFrontend.Tests/WeatherGetterFrontend.Tests.csproj

# End-to-end smoke tests
dotnet test WeatherGetterFrontend.E2E/WeatherGetterFrontend.E2E.csproj
```

### Playwright Setup (First Run)

Install Chromium for Playwright before running E2E tests for the first time:

```bash
pwsh WeatherGetterFrontend.E2E/bin/Debug/net8.0/playwright.ps1 install chromium
```

##  License

BSD 3-Clause
