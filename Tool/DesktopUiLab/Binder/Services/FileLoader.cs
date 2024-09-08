namespace Binder.Services;

using System;
using System.Diagnostics;
using System.IO;
using Binder.Models;
using Binder.ViewModels;
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

        // 리스트업이 끝난 후 filter 설정을 위해 한번 호출
        this.vmHome.SearchKeyword = string.Empty;
    }
}
