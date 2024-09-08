namespace Binder.Services;

using System;
using System.Diagnostics;
using Binder.ViewModels;
using Cs.Logging;
using Microsoft.Extensions.Configuration;

internal sealed class FileLoader(VmHome vmHome, IConfiguration config)
{
    private readonly VmHome vmHome = vmHome;

    public void Load()
    {
        var path = config.GetValue<string>("BindFilePath") ?? throw new Exception("BindFilePath is not set in the configuration file.");
        Log.Debug($"Loading files from {path}");
    }
}
