using System.Text.Json;
using FluentAssertions;
using Ocelot.Configuration.File;
using Xunit;

namespace JobLess.Tests.ApiGateway;

public class OcelotConfigTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private static readonly string[] ExpectedUpstreamPrefixes =
    {
        "/api/clients/{everything}",
        "/api/Advertisements/{everything}",
        "/api/Companies/{everything}",
        "/api/notifications/{everything}",
        "/api/job-applications/{everything}",
        "/api/Auth/{everything}",
        "/api/{everything}",
    };

    [Theory]
    [InlineData("ocelot.Local.json")]
    [InlineData("ocelot.Development.json")]
    public void OcelotFile_Deserializes_WithExpectedRoutes(string fileName)
    {
        var path = Path.Combine(AppContext.BaseDirectory, fileName);

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<FileConfiguration>(json, JsonOptions);

        config.Should().NotBeNull();
        config!.Routes.Should().HaveCount(ExpectedUpstreamPrefixes.Length);
        config.Routes.Select(r => r.UpstreamPathTemplate).Should().BeEquivalentTo(ExpectedUpstreamPrefixes);

        foreach (var route in config.Routes)
        {
            route.DownstreamPathTemplate.Should().NotBeNullOrWhiteSpace();
            route.DownstreamHostAndPorts.Should().ContainSingle();
            route.DownstreamHostAndPorts[0].Host.Should().NotBeNullOrWhiteSpace();
            route.DownstreamHostAndPorts[0].Port.Should().BeGreaterThan(0);
            route.RateLimitOptions.EnableRateLimiting.Should().BeTrue();
        }

        config.GlobalConfiguration.BaseUrl.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LocalAndDevelopmentConfigs_TargetSameUpstreamRoutesInSameOrder()
    {
        var localPath = Path.Combine(AppContext.BaseDirectory, "ocelot.Local.json");
        var devPath = Path.Combine(AppContext.BaseDirectory, "ocelot.Development.json");

        var local = JsonSerializer.Deserialize<FileConfiguration>(File.ReadAllText(localPath), JsonOptions)!;
        var dev = JsonSerializer.Deserialize<FileConfiguration>(File.ReadAllText(devPath), JsonOptions)!;

        local.Routes.Select(r => r.UpstreamPathTemplate)
            .Should().Equal(dev.Routes.Select(r => r.UpstreamPathTemplate));
    }
}
