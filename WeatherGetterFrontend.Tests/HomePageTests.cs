namespace WeatherGetterFrontend.Tests
{
    using Bunit;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using RichardSzalay.MockHttp;
    using WeatherGetterFrontend.Components.Pages;

    public class HomePageTests : BunitContext
    {
        [Fact]
        public void Home_ShouldRenderWelcomeHeading()
        {
            var cut = Render<Home>();

            cut.Find("h1").TextContent.Should().Be("Welcome to Weather Info site");
        }

        [Fact]
        public void Cities_ShouldRenderFetchedLocations()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When(HttpMethod.Get, "http://localhost/weather/Get-AllCities")
                .Respond("application/json", "[{\"id\":1,\"city\":\"Warsaw\",\"latitude\":52.23,\"longitude\":21.01}]");

            Services.AddSingleton(new HttpClient(mockHttp) { BaseAddress = new Uri("http://localhost") });

            var cut = Render<Cities>();

            cut.WaitForAssertion(() =>
            {
                cut.Markup.Should().Contain("Warsaw");
                cut.FindAll("tbody tr").Count.Should().Be(1);
            });

            mockHttp.VerifyNoOutstandingExpectation();

        }
    }
}