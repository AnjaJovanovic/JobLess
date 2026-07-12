using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace JobLess.Tests.ApiGateway;

public class GatewayRoutingTests : IAsyncLifetime
{
    private IHost? _fakeDownstream;
    private int _fakeDownstreamPort;

    public async Task InitializeAsync()
    {
        _fakeDownstream = Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls("http://127.0.0.1:0");
                webBuilder.Configure(app => app.Run(context =>
                {
                    context.Response.Headers["X-Fake-Downstream"] = "hit";
                    return context.Response.WriteAsync("fake-client-response");
                }));
            })
            .Build();

        await _fakeDownstream.StartAsync();

        var address = _fakeDownstream.Services
            .GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!
            .Addresses.First();

        _fakeDownstreamPort = new Uri(address).Port;
    }

    public async Task DisposeAsync()
    {
        if (_fakeDownstream is not null)
        {
            await _fakeDownstream.StopAsync();
            _fakeDownstream.Dispose();
        }
    }

    [Fact]
    public async Task Gateway_Proxies_ClientsRoute_ToDownstreamService()
    {
        await using var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureAppConfiguration((_, config) =>
                {
                    config.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Routes:0:DownstreamHostAndPorts:0:Host"] = "127.0.0.1",
                        ["Routes:0:DownstreamHostAndPorts:0:Port"] = _fakeDownstreamPort.ToString(),
                    });
                });
            });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/clients/ping");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.Should().ContainKey("X-Fake-Downstream");

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Be("fake-client-response");
    }
}
