namespace CutEditor.Services;

using System;
using System.Collections.Generic;
using System.IO;
using Cs.Logging;
using CutEditor.Model;
using CutEditor.ViewModel;
using Du.Core.Util;
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

        IList<CutScene> list = new List<CutScene>();
        using var document = JsonHelper.LoadJsonc(path);
        document.RootElement.GetArray("Data", in list, e => new CutScene(e));

        vmHome.AddCutScenes(list);

        Log.Info($"cutscene loading finished. {list.Count} cutscenes loaded.");
    }
}
