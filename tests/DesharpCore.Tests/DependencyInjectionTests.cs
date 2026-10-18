using System;
using System.Collections.Generic;
using DesharpCore;
using DesharpCore.Core;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DesharpCore.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void TestAddDesharpRegistersServices()
    {
        var services = new ServiceCollection();
        services.AddDesharp();

        var serviceProvider = services.BuildServiceProvider();

        var desharpService = serviceProvider.GetService<IDesharpService>();
        var desharp = serviceProvider.GetService<IDesharp>();

        Assert.NotNull(desharpService);
        Assert.NotNull(desharp);
        Assert.Same(desharpService, desharp);
    }

    [Fact]
    public void TestAddDesharpWithConfigurationAction()
    {
        var services = new ServiceCollection();
        services.AddDesharp(cfg =>
        {
            cfg.Enabled = true;
            cfg.Depth = 5;
            cfg.LogFormat = LogFormat.Text;
            cfg.SourceLocation = false;
        });

        var serviceProvider = services.BuildServiceProvider();
        var desharpService = serviceProvider.GetRequiredService<IDesharpService>();

        Assert.NotNull(desharpService);
        Assert.True(desharpService.Enabled());
    }

    [Fact]
    public void TestAddDesharpWithConfigurationStruct()
    {
        var services = new ServiceCollection();
        services.AddDesharp(new DebugConfig
        {
            Enabled = true,
            Depth = 4,
            LogFormat = LogFormat.Text
        });

        var serviceProvider = services.BuildServiceProvider();
        var desharpService = serviceProvider.GetRequiredService<IDesharpService>();

        Assert.NotNull(desharpService);
        Assert.True(desharpService.Enabled());
    }

    [Fact]
    public void TestAddDesharpCoreAliases()
    {
        var services = new ServiceCollection();
        services.AddDesharpCore();

        var serviceProvider = services.BuildServiceProvider();
        var desharpService = serviceProvider.GetService<IDesharpService>();
        var desharp = serviceProvider.GetService<IDesharp>();

        Assert.NotNull(desharpService);
        Assert.NotNull(desharp);
        Assert.Same(desharpService, desharp);

        var services2 = new ServiceCollection();
        services2.AddDesharpCore(cfg => cfg.Enabled = true);
        var sp2 = services2.BuildServiceProvider();
        Assert.NotNull(sp2.GetService<IDesharpService>());

        var services3 = new ServiceCollection();
        services3.AddDesharpCore(new DebugConfig { Enabled = true });
        var sp3 = services3.BuildServiceProvider();
        Assert.NotNull(sp3.GetService<IDesharpService>());
    }

    [Fact]
    public void TestDesharpServiceMethods()
    {
        var services = new ServiceCollection();
        services.AddDesharp();

        var serviceProvider = services.BuildServiceProvider();
        var desharp = serviceProvider.GetRequiredService<IDesharp>();

        desharp.Enabled(true);
        Assert.True(desharp.Enabled());

        var list = new List<string> { "item1", "item2" };
        string dumped = desharp.Dump(list, new DumpOptions { Return = true });
        Assert.NotNull(dumped);
        Assert.Contains("item1", dumped);

        string dumpedArgs = desharp.Dump("arg1", 123);
        Assert.NotNull(dumpedArgs);

        desharp.Assert(true, "DI Assert test");
        desharp.Log("DI Log info test", Level.INFO);
        desharp.Log(new InvalidOperationException("DI test exception"));

        double timer = desharp.Timer("di-timer", true);
        Assert.True(timer >= 0);

        double procTime = desharp.GetProcessingTime();
        Assert.True(procTime >= 0);

        desharp.Configure(new DebugConfig
        {
            Enabled = true,
            Depth = 3
        });
    }

    public class SampleConsumerService
    {
        private readonly IDesharp _desharp;

        public SampleConsumerService(IDesharp desharp)
        {
            _desharp = desharp;
        }

        public string ProcessData(object data)
        {
            return _desharp.Dump(data, new DumpOptions { Return = true });
        }
    }

    public class SampleMvcController
    {
        private readonly IDesharpService _desharpService;

        public SampleMvcController(IDesharpService desharpService)
        {
            _desharpService = desharpService;
        }

        public string Index()
        {
            var model = new { Controller = "Home", Action = "Index", Status = 200 };
            return _desharpService.Dump(model, new DumpOptions { Return = true });
        }
    }

    [Fact]
    public void TestDependencyInjectionInMvcController()
    {
        var services = new ServiceCollection();
        services.AddDesharp();
        services.AddTransient<SampleMvcController>();

        var serviceProvider = services.BuildServiceProvider();
        var controller = serviceProvider.GetRequiredService<SampleMvcController>();

        string result = controller.Index();
        Assert.NotNull(result);
        Assert.Contains("Home", result);
        Assert.Contains("Index", result);
        Assert.Contains("200", result);
    }
}
