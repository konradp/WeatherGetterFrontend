# WeatherGetterFrontend

A modern weather information web application built with ASP.NET Core 8.0 and Blazor Server components. This application communicates directly with backend app [WeatherGetter](https://github.com/konradp/WeatherGetter)

## Features

- View current weather data for various locations
- Browse and manage different cities
- Compare weather conditions between multiple locations
- Visualize weather data using Chart.js
- Bootstrap-powered responsive UI

## Presentation

<a href="https://www.youtube.com/watch?v=LvGLFAnlJyU" target="_blank">Watch the project presentation on YouTube</a>

## Technologies

- ASP.NET Core 8.0
- Blazor Server Components
- Bootstrap
- Chart.js v3.7.1
- C# with .NET 8.0
- CSS with Bootstrap

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

##  License

BSD 3-Clause
