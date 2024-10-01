namespace CutEditor.Services;

using System;
using System.Collections.Generic;
using System.IO;
using Cs.Core.Util;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel;
using Microsoft.Extensions.Configuration;

internal sealed class FileLoader(VmHome vmHome, IConfiguration config)
{
    public void Load()
    {
        var path = config.GetValue<string>("CutSceneDataFile") ?? throw new Exception("CutSceneDataFile is not set in the configuration file.");
        if (!File.Exists(path))
        {
            Log.ErrorAndExit($"cutscene file not found: {path}");
        }

        Log.Debug($"Loading files from {path}");

        var list = new List<CutScene>();
        var json = JsonUtil.Load(path);
        json.GetArray("Data", list, (e, i) => new CutScene(e));

        vmHome.AddCutScenes(list);

        Log.Info($"cutscene loading finished. {list.Count} cutscenes loaded.");
    }
}
