﻿namespace Binder.Services;

using System;
using System.IO;
using Binder.Model;
using Binder.ViewModel;
using Cs.Logging;
using Microsoft.Extensions.Configuration;

internal sealed class FileLoader(VmHome vmHome, IConfiguration config)
{
    private readonly VmHome vmHome = vmHome;

    public void Load()
    {
        var path = config.GetValue<string>("BindFilePath") ?? throw new Exception("BindFilePath is not set in the configuration file.");
        if (!Directory.Exists(path))
        {
            Log.ErrorAndExit($"Directory not found: {path}");
        }

        Log.Debug($"Loading files from {path}");

        foreach (var file in Directory.EnumerateFiles(path, "*.jsonc", SearchOption.AllDirectories))
        {
            Log.Debug($"Loading file: {file}");

            if (file.Contains("Schema.jsonc"))
            {
                continue;
            }

            try
            {
                var bindFile = new BindFile(file);
                this.vmHome.AddFile(bindFile);
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to load file:{file} exception:{ex.Message}");
            }
        }
    }
}
