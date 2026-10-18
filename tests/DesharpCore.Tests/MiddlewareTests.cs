using DesharpCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DesharpCore.Tests;

[CollectionDefinition("Web environment", DisableParallelization = true)]
public class WebEnvironmentCollection
{
}

[Collection("Web environment")]
public class MiddlewareTests : IDisposable
{
    public void Dispose()
    {
        // the middleware switches the process-wide environment to web, restore it for other tests
        Debug.Configure(new DebugConfig { EnvType = EnvType.Windows });
    }

    private static async Task<IHost> CreateHostAsync(RequestDelegate endpoint)
    {
        return await new HostBuilder()
            .ConfigureWebHost(web => web
                .UseTestServer()
                .ConfigureServices(services => services.AddDesharp(new DebugConfig { Enabled = true }))
                .Configure(app =>
                {
                    app.Use((context, next) =>
                    {
                        context.Connection.RemoteIpAddress = System.Net.IPAddress.Loopback;
                        return next();
                    });
                    app.UseDesharp();
                    app.Run(endpoint);
                }))
            .StartAsync();
    }

    [Fact]
    public async Task HtmlResponseGetsDebugBar()
    {
        using var host = await CreateHostAsync(async context =>
        {
            context.Response.ContentType = "text/html";
            Debug.Dump("middleware-dump-value");
            await context.Response.WriteAsync("<html><body>hello</body></html>");
        });
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add("Accept", "text/html");

        string body = await client.GetStringAsync("/");

        Assert.Contains("hello", body);
        Assert.Contains("Desharp.GetInstance()", body);
        Assert.Contains("Dumps (1)", body);
    }

    [Fact]
    public async Task UnhandledExceptionIsRenderedAsInternalServerError()
    {
        using var host = await CreateHostAsync(_ => throw new InvalidOperationException("boom-from-endpoint"));
        var client = host.GetTestClient();
        client.DefaultRequestHeaders.Add("Accept", "text/html");

        var response = await client.GetAsync("/");
        string body = await response.Content.ReadAsStringAsync();

        Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Contains("boom-from-endpoint", body);
    }
}
