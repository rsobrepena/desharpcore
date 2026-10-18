using System;
using System.Collections.Generic;
using System.Data;
using DesharpCore;
using DesharpCore.Core;
using DesharpCore.Panels;
using DesharpCore.Renderers;
using Xunit;

namespace DesharpCore.Tests;

public class DesharpCoreTests
{
    public DesharpCoreTests()
    {
        Debug.Enabled(true);
    }

    [Fact]
    public void TestDumpList()
    {
        var list = new List<int?>() { 100, 200, null };
        string dumped = Debug.Dump(list, new DumpOptions { Return = true });
        Assert.NotNull(dumped);
        Assert.Contains("List<Int32?>", dumped);
        Assert.Contains("100", dumped);
        Assert.Contains("200", dumped);
        Assert.Contains("null", dumped);
    }

    [Fact]
    public void TestDumpException()
    {
        try
        {
            throw new InvalidOperationException("Something went wrong!");
        }
        catch (Exception ex)
        {
            string dumped = Debug.Dump(ex, new DumpOptions { Return = true });
            Assert.NotNull(dumped);
            Assert.Contains("InvalidOperationException", dumped);
            Assert.Contains("Something went wrong!", dumped);
        }
    }

    [Fact]
    public void TestDumpObjectWithCustomDepth()
    {
        var complexObj = new
        {
            Name = "DesharpCore",
            Version = 10,
            Nested = new
            {
                Key = "Value",
                Inner = new
                {
                    Deep = true
                }
            }
        };

        string dumped = Debug.Dump(complexObj, new DumpOptions { Depth = 5, Return = true });
        Assert.NotNull(dumped);
        Assert.Contains("DesharpCore", dumped);
        Assert.Contains("Deep", dumped);
    }

    [Fact]
    public void TestDumpDictionaryAndDataTypes()
    {
        var dict = new Dictionary<string, object>
        {
            { "str", "hello" },
            { "dt", DateTime.UtcNow },
            { "guid", Guid.NewGuid() },
            { "dec", 123.45m }
        };

        string dumped = Debug.Dump(dict, new DumpOptions { Return = true });
        Assert.NotNull(dumped);
        Assert.Contains("Dictionary<String,Object>", dumped);
        Assert.Contains("hello", dumped);
    }

    [Fact]
    public void TestDumpDataTable()
    {
        var dt = new DataTable("TestTable");
        dt.Columns.Add("Id", typeof(int));
        dt.Columns.Add("Name", typeof(string));
        dt.Rows.Add(1, "Alice");
        dt.Rows.Add(2, "Bob");

        string dumped = Debug.Dump(dt, new DumpOptions { Return = true });
        Assert.NotNull(dumped);
        Assert.Contains("DataTable", dumped);
        Assert.Contains("Alice", dumped);
        Assert.Contains("Bob", dumped);
    }

    [Fact]
    public void TestToolsMethods()
    {
        string jsEscaped = Tools.JavascriptString("Hello <world> & \"everyone\"");
        Assert.NotNull(jsEscaped);
        Assert.Contains("Hello <world> &", jsEscaped);

        string md5 = Tools.Md5("test");
        Assert.Equal("098f6bcd4621d373cade4e832627b4f6", md5);

        string indented = Tools.SpaceIndent(4, false);
        Assert.Equal("    ", indented);

        string relPath = Tools.RelativeSourceFullPath("/some/path/file.cs");
        Assert.NotNull(relPath);
    }

    [Fact]
    public void TestTimerAndProcessingTime()
    {
        double timerVal = Debug.Timer("unit-test-timer", true);
        Assert.True(timerVal >= 0);

        double procTime = Debug.GetProcessingTime();
        Assert.True(procTime >= 0);
    }

    [Fact]
    public void TestConfigure()
    {
        Debug.Configure(new DebugConfig
        {
            Enabled = true,
            Depth = 4,
            LogFormat = LogFormat.Text,
            SourceLocation = false
        });

        Assert.True(Debug.Enabled());
    }

    [Fact]
    public void TestAssertAndLog()
    {
        Debug.Assert(true, "Condition passed", Level.INFO);
        Debug.Log("Information message", Level.INFO);
        Debug.Log(new InvalidOperationException("Test log exception"));
    }

    [Fact]
    public void TestPanelsInstantiation()
    {
        var dumpsPanel = new DesharpCore.Panels.Dumps();
        Assert.Equal("dumps", dumpsPanel.Name);

        var exceptionsPanel = new DesharpCore.Panels.Exceptions();
        Assert.Equal("exceptions", exceptionsPanel.Name);

        var routingPanel = new DesharpCore.Panels.Routing();
        Assert.Equal("routing", routingPanel.Name);

        var sessionPanel = new DesharpCore.Panels.Session();
        Assert.Equal("session", sessionPanel.Name);

        var sysInfoPanel = new DesharpCore.Panels.SystemInfo();
        Assert.Equal("systeminfo", sysInfoPanel.Name);
    }

    [Fact]
    public void TestAssetsLoading()
    {
        Assert.False(string.IsNullOrEmpty(Assets.bar_css));
        Assert.False(string.IsNullOrEmpty(Assets.dumps_css));
        Assert.False(string.IsNullOrEmpty(Assets.exception_css));
        Assert.False(string.IsNullOrEmpty(Assets.logs_css));
    }

    [Fact]
    public void TestSessionKey()
    {
        Assert.Equal("$$$DesharpCore", Debug.SESSION_STORAGE_KEY);
    }
}
