namespace WeatherGetterFrontend.E2E
{
    using System.Diagnostics;
    using System.Text;
    using FluentAssertions;
    using Microsoft.Playwright;

    public class HomePageTests
    {
        [Fact]
        public async Task HomePage_ShouldRenderWelcomeHeading()
        {
            var solutionRoot = FindSolutionRoot();
            var appUrl = "http://127.0.0.1:5188";
            using var app = StartApplication(solutionRoot, appUrl);

            try
            {
                await WaitUntilAppIsReady(appUrl, TimeSpan.FromSeconds(30));

                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

                var page = await browser.NewPageAsync();
                await page.GotoAsync(appUrl, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });

                var headingText = await page.Locator("h1").First.InnerTextAsync();
                headingText.Should().Be("Welcome to Weather Info site");
            }
            finally
            {
                if (!app.HasExited)
                {
                    app.Kill(entireProcessTree: true);
                    app.WaitForExit(5000);
                }
            }
        }

        private static string FindSolutionRoot()
        {
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current is not null)
            {
                var slnPath = Path.Combine(current.FullName, "WeatherGetterFrontend.sln");
                if (File.Exists(slnPath))
                {
                    return current.FullName;
                }

                current = current.Parent;
            }

            throw new DirectoryNotFoundException("Could not locate WeatherGetterFrontend.sln from test output directory.");
        }

        private static Process StartApplication(string solutionRoot, string appUrl)
        {
            var projectPath = Path.Combine(solutionRoot, "WeatherGetterFrontend.csproj");

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --urls {appUrl}",
                WorkingDirectory = solutionRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            startInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
            startInfo.Environment["ApiBaseUrl"] = "http://127.0.0.1:5999";

            var process = new Process { StartInfo = startInfo };
            var output = new StringBuilder();

            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    output.AppendLine(e.Data);
                }
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    output.AppendLine(e.Data);
                }
            };

            if (!process.Start())
            {
                throw new InvalidOperationException("Failed to start application process.");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }

        private static async Task WaitUntilAppIsReady(string appUrl, TimeSpan timeout)
        {
            using var http = new HttpClient();
            var start = DateTime.UtcNow;
            Exception? lastError = null;

            while (DateTime.UtcNow - start < timeout)
            {
                try
                {
                    using var response = await http.GetAsync(appUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    lastError = ex;
                }

                await Task.Delay(500);
            }

            throw new TimeoutException($"Application did not become ready at {appUrl}. Last error: {lastError?.Message}");
        }
    }
}