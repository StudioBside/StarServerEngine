namespace CutEditor.Model;

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

public sealed class AssetList
{
    private readonly List<string> bgmFiles = new();
    private readonly List<string> sfxFiles = new();
    private readonly List<string> voiceFils = new();
    private readonly List<string> unitMotions = new();

    public AssetList(IConfiguration config)
    {
        Instance = this;

        var bgmRoot = config["BgmRoot"] ?? throw new Exception($"BgmRoot is not defined in the configuration file.");
        var sfxRoot = config["SfxRoot"] ?? throw new Exception($"SfxRoot is not defined in the configuration file.");
        var voiceRoot = config["VoiceRoot"] ?? throw new Exception($"VoiceRoot is not defined in the configuration file.");

        this.unitMotions.Add(null!);
        foreach (var data in config.GetSection("UnitMotionList").GetChildren())
        {
            if (data.Value is not null)
            {
                this.unitMotions.Add(data.Value);
            }
        }

        foreach (var file in Directory.GetFiles(bgmRoot, "*.mp3", SearchOption.AllDirectories))
        {
            this.bgmFiles.Add(file);
        }

        foreach (var file in Directory.GetFiles(sfxRoot, "*.wav", SearchOption.AllDirectories))
        {
            this.sfxFiles.Add(file);
        }
    }

    public static AssetList Instance { get; private set; } = null!;

    public IReadOnlyList<string> BgmFiles => this.bgmFiles;
    public IReadOnlyList<string> SfxFiles => this.sfxFiles;
    public IReadOnlyList<string> VoiceFiles => this.voiceFils;
    public IReadOnlyList<string> UnitMotions => this.unitMotions;
}
