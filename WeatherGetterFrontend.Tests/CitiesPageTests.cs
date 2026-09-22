namespace WeatherGetterFrontend.Tests
{
    using System.Net;
    using Bunit;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using RichardSzalay.MockHttp;
    using WeatherGetterFrontend.Components.Pages;

    public class CitiesPageTests : BunitContext
    {
        [Fact]
        public async Task Cities_AddCity_WithEmptyName_ShouldNotSendPost()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "[{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01}]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });

            var cut = Render<Cities>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Warsaw"));

            await cut.InvokeAsync(() => cut.FindAll("button[type=submit]")[0].Click());

            cut.Markup.Should().Contain("Warsaw");
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Cities_AddCity_WithValidName_ShouldRefreshCityList()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "[{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01}]");
            mockHttp.Expect(HttpMethod.Post, "http://localhost/weather/Post-CityToDB?cityName=Krakow")
                .Respond(HttpStatusCode.OK);
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "["
                    + "{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01},"
                    + "{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}"
                    + "]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });

            var cut = Render<Cities>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Warsaw"));

            await cut.InvokeAsync(() => cut.Find("input").Change("Krakow"));
            await cut.InvokeAsync(() => cut.FindAll("button[type=submit]")[0].Click());

            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain("Krakow");
                cut.FindAll("tbody tr").Count.Should().Be(2);
            });

            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Cities_RemoveCity_WhenUserCancelsConfirmation_ShouldNotDelete()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "["
                    + "{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01},"
                    + "{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}"
                    + "]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });
            JSInterop.Mode = JSRuntimeMode.Strict;
            JSInterop.Setup<bool>("confirm", _ => true).SetResult(false);

            var cut = Render<Cities>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Krakow"));

            await cut.InvokeAsync(() => cut.Find("select.form-select").Change("Warsaw"));
            await cut.InvokeAsync(() => cut.FindAll("button[type=submit]")[1].Click());

            JSInterop.Invocations.Count(x => x.Identifier == "confirm").Should().Be(1);
            cut.Markup.Should().Contain("Warsaw");
            cut.FindAll("tbody tr").Count.Should().Be(2);
            mockHttp.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task Cities_RemoveCity_WhenUserConfirms_ShouldDeleteAndRefreshList()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "["
                    + "{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01},"
                    + "{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}"
                    + "]");
            mockHttp.Expect(HttpMethod.Delete, "http://localhost/weather/Delete-City?cityName=Warsaw")
                .Respond(HttpStatusCode.OK);
            mockHttp.Expect(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "[{\"id\":2,\"city\":\"Krakow\",\"latitude\":50.06,\"longitude\":19.94}]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });
            JSInterop.Mode = JSRuntimeMode.Strict;
            JSInterop.Setup<bool>("confirm", _ => true).SetResult(true);

            var cut = Render<Cities>();
            cut.WaitForAssertion(() => cut.Markup.Should().Contain("Krakow"));

            await cut.InvokeAsync(() => cut.Find("select.form-select").Change("Warsaw"));
            await cut.InvokeAsync(() => cut.FindAll("button[type=submit]")[1].Click());

            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().NotContain("<td>1</td>");
                cut.FindAll("tbody tr").Count.Should().Be(1);
            });

            JSInterop.Invocations.Count(x => x.Identifier == "confirm").Should().Be(1);
            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
