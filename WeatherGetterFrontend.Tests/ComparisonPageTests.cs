namespace WeatherGetterFrontend.Tests
{
    using System.Reflection;
    using Bunit;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using RichardSzalay.MockHttp;
    using WeatherGetterFrontend.Components.Pages;

    public class ComparisonPageTests : BunitContext
    {
        [Fact]
        public void Comparison_WhenOnlyOneCitySelected_ShouldNotInvokeChartJs()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "["
                    + "{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01},"
                    + "{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}"
                    + "]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });
            JSInterop.SetupVoid("weatherChartComparison.render", _ => true);

            var cut = Render<Comparison>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Krakow"));

            cut.InvokeAsync(() => cut.FindAll("select.form-select")[0].Change("Warsaw"));
            cut.InvokeAsync(() => cut.Find("button").Click());

            JSInterop.Invocations.Count(x => x.Identifier == "weatherChartComparison.render").Should().Be(0);
        }

        [Fact]
        public async Task Comparison_WithBothCitiesSelected_ShouldInvokeChartJs()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "["
                    + "{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01},"
                    + "{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}"
                    + "]");

            mockHttp.When(HttpMethod.Get, "http://localhost/weather/Get-ByCity?cityName=Warsaw")
                .Respond("application/json", "["
                    + "{\"id\":1,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:00:00\",\"temperatureC\":5.0},"
                    + "{\"id\":2,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:15:00\",\"temperatureC\":5.5},"
                    + "{\"id\":3,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:30:00\",\"temperatureC\":6.0}"
                    + "]");

            mockHttp.When(HttpMethod.Get, "http://localhost/weather/Get-ByCity?cityName=Krakow")
                .Respond("application/json", "["
                    + "{\"id\":1,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:00:00\",\"temperatureC\":4.0},"
                    + "{\"id\":2,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:15:00\",\"temperatureC\":4.5},"
                    + "{\"id\":3,\"location\":null,\"dateOnly\":\"2026-01-01\",\"timeOnly\":\"10:30:00\",\"temperatureC\":5.0}"
                    + "]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });
            JSInterop.Mode = JSRuntimeMode.Loose;

            var cut = Render<Comparison>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Krakow"));

            var selectedCityField = typeof(Comparison).GetField("selectedCityName", BindingFlags.Instance | BindingFlags.NonPublic);
            selectedCityField.Should().NotBeNull();
            selectedCityField!.SetValue(cut.Instance, "Warsaw");

            var comparisonCityField = typeof(Comparison).GetField("comparisonCityName", BindingFlags.Instance | BindingFlags.NonPublic);
            comparisonCityField.Should().NotBeNull();
            comparisonCityField!.SetValue(cut.Instance, "Krakow");

            cut.Instance.StartDate = new DateTime(2026, 1, 1);
            cut.Instance.EndDate = new DateTime(2026, 1, 1);
            cut.Instance.StartHour = 10;
            cut.Instance.StartMinute = 0;
            cut.Instance.EndHour = 10;
            cut.Instance.EndMinute = 30;
            cut.Render();

            await cut.InvokeAsync(() => cut.Find("button").Click());

            cut.WaitForAssertion(() =>
            {
                var renderChartField = typeof(Comparison).GetField("renderChart", BindingFlags.Instance | BindingFlags.NonPublic);
                renderChartField.Should().NotBeNull();
                var renderChart = renderChartField!.GetValue(cut.Instance) as bool?;
                renderChart.Should().BeTrue();
            });
        }
    }
}