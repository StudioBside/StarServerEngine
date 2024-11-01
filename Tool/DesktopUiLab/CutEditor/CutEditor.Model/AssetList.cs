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
    private readonly List<string> bgImageFiles = new();
    private readonly List<string> storyImageFiles = new();
    private readonly List<string> movFiles = new();
    private readonly List<string> slateFiles = new();
    private readonly List<string> spineFiles = new();

    public AssetList(IConfiguration config)
    {
        Instance = this;

        var bgmRoot = config["BgmRoot"] ?? throw new Exception($"BgmRoot is not defined in the configuration file.");
        var sfxRoot = config["SfxRoot"] ?? throw new Exception($"SfxRoot is not defined in the configuration file.");
        var voiceRoot = config["VoiceRoot"] ?? throw new Exception($"VoiceRoot is not defined in the configuration file.");
        var bgImageRoot = config["BgImageRoot"] ?? throw new Exception($"BgImageRoot is not defined in the configuration file.");
        var bgImage2Root = config["BgImage2Root"] ?? throw new Exception($"BgImage2Root is not defined in the configuration file.");
        var movRoot = config["MovRoot"] ?? throw new Exception($"MovRoot is not defined in the configuration file.");
        var slateRoot = config["SlateRoot"] ?? throw new Exception($"SlateRoot is not defined in the configuration file.");
        var spineRoot = config["SpineRoot"] ?? throw new Exception($"SpineRoot is not defined in the configuration file.");

        this.unitMotions.Add(null!);
        foreach (var data in config.GetSection("UnitMotionList").GetChildren())
        {
            if (data.Value is not null)
            {
                this.unitMotions.Add(data.Value);
            }
        }

        if (Directory.Exists(bgmRoot))
        {
            foreach (var file in Directory.GetFiles(bgmRoot, "*.mp3", SearchOption.AllDirectories))
            {
                this.bgmFiles.Add(file);
            }
        }

        if (Directory.Exists(sfxRoot))
        {
            foreach (var file in Directory.GetFiles(sfxRoot, "*.wav", SearchOption.AllDirectories))
            {
                var fileName = Path.GetFileName(file);
                if (fileName.StartsWith("SFX_CUTSCENE_") == false)
                {
                    continue;
                }

                this.sfxFiles.Add(file);
            }
        }

        if (Directory.Exists(voiceRoot))
        {
            foreach (var file in Directory.GetFiles(voiceRoot, "*.ogg", SearchOption.AllDirectories))
            {
                this.voiceFils.Add(file);
            }
        }

        if (Directory.Exists(bgImageRoot))
        {
            foreach (var file in Directory.GetFiles(bgImageRoot, "*.png", SearchOption.AllDirectories))
            {
                this.bgImageFiles.Add(file);
            }
        }

        if (Directory.Exists(bgImage2Root))
        {
            foreach (var file in Directory.GetFiles(bgImage2Root, "*.png", SearchOption.AllDirectories))
            {
                this.storyImageFiles.Add(file);
            }
        }

        if (Directory.Exists(movRoot))
        {
            foreach (var file in Directory.GetFiles(movRoot, "*.mp4", SearchOption.AllDirectories))
            {
                this.movFiles.Add(file);
            }
        }

        if (Directory.Exists(slateRoot))
        {
            foreach (var file in Directory.GetFiles(slateRoot, "*.prefab", SearchOption.AllDirectories))
            {
                this.slateFiles.Add(file);
            }
        }

        if (Directory.Exists(spineRoot))
        {
            foreach (var file in Directory.GetFiles(spineRoot, "*.prefab", SearchOption.AllDirectories))
            {
                this.spineFiles.Add(file);
            }
        }
    }

    public static AssetList Instance { get; private set; } = null!;

    public IReadOnlyList<string> BgmFiles => this.bgmFiles;
    public IReadOnlyList<string> SfxFiles => this.sfxFiles;
    public IReadOnlyList<string> VoiceFiles => this.voiceFils;
    public IReadOnlyList<string> UnitMotions => this.unitMotions;
    public IReadOnlyList<string> BgImageFiles => this.bgImageFiles;
    public IReadOnlyList<string> StoryImageFiles => this.storyImageFiles;
    public IReadOnlyList<string> MovFiles => this.movFiles;
    public IReadOnlyList<string> SlateFiles => this.slateFiles;
    public IReadOnlyList<string> SpineFiles => this.spineFiles;
}
