namespace CutEditor.Model;

using System;
using System.Collections.Generic;
using Cs.Core.Util;
using Cs.Logging;
using Microsoft.Extensions.Configuration;

public sealed class UnitContainer
{
    private readonly Dictionary<int, Unit> units = new();

    public UnitContainer(IConfiguration config)
    {
        var templetRoot = config["TempletDataRoot"] ?? throw new Exception("TempletDataRoot is not set in the configuration file.");
        var unitImageRoot = config["UnitImageRoot"] ?? throw new Exception("UnitImageRoot is not set in the configuration file.");
        // get filenames array 
        var filenames = config.GetSection("UnitTempletNames").GetChildren();
        foreach (var filename in filenames)
        {
            var path = Path.Combine(templetRoot, filename.Value ?? string.Empty);
            Log.Debug($"unit templet file: {path}");

            if (!File.Exists(path))
            {
                Log.ErrorAndExit($"unit file not found: {path}");
            }

            var json = JsonUtil.Load(path);
            var units = new List<Unit>();
            json.GetArray("Data", units, (e, i) => new Unit(e, unitImageRoot));

            Log.Info($"unit loading finished. {this.units.Count} units loaded.");

            foreach (var unit in units)
            {
                this.units.Add(unit.Id, unit);
            }
        }

        Log.Debug($"unit templet loading finished. {this.units.Count} units loaded.");
    }

    public IEnumerable<Unit> Units => this.units.Values;
}
