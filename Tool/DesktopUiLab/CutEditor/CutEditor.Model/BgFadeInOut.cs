namespace CutEditor.Model;

using System.ComponentModel;
using System.Drawing;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Cs.Math;
using CutEditor.Model.Detail;
using Newtonsoft.Json.Linq;
using static CutEditor.Model.Enums;

public sealed class BgFadeInOut : ObservableObject
{
    private BgFadeType fadeType;
    private Color startColor = Color.FromArgb(alpha: 0, Color.Black);
    private Color endColor = Color.FromArgb(alpha: 255, Color.Black);
    private float time = 0.5f;

    public BgFadeType FadeType
    {
        get => this.fadeType;
        set => this.SetProperty(ref this.fadeType, value);
    }

    public Color StartColor
    {
        get => this.startColor;
        set => this.SetProperty(ref this.startColor, value);
    }

    public Color EndColor
    {
        get => this.endColor;
        set => this.SetProperty(ref this.endColor, value);
    }

    public float Time
    {
        get => this.time;
        set => this.SetProperty(ref this.time, value);
    }

    public string ButtonText => $"{this.fadeType.ToString().Substring(4)} {this.time:0.##}";

    public static BgFadeInOut? Create(JToken token)
    {
        var time = token.GetFloat("BgFadeInTime", 0f);
        if (time.IsNearlyZero() == false)
        {
            return new BgFadeInOut
            {
                time = time,
                fadeType = BgFadeType.FadeIn,
                startColor = JsonLoadHelper.LoadColor(token, "BgFadeInStartCol") ?? Color.White,
                endColor = JsonLoadHelper.LoadColor(token, "BgFadeInCol") ?? Color.White,
            };
        }

        time = token.GetFloat("BgFadeOutTime", 0f);
        if (time.IsNearlyZero() == false)
        {
            return new BgFadeInOut
            {
                time = time,
                fadeType = BgFadeType.FadeOut,
                startColor = Color.White, // no start color for fade out
                endColor = JsonLoadHelper.LoadColor(token, "BgFadeOutCol") ?? Color.White,
            };
        }

        return null;
    }

    public BgFadeInOut Clone()
    {
        return this.MemberwiseClone() as BgFadeInOut ?? throw new Exception("Failed to clone BgFadeInOut");
    }

    public override string ToString()
    {
        // color to hex
        var startColor = this.StartColor.ToArgb().ToString("X");
        var endColor = this.EndColor.ToArgb().ToString("X");

        return this.fadeType switch
        {
            BgFadeType.FadeIn => $"{this.FadeType} {this.Time}초 {startColor} -> {endColor}",
            BgFadeType.FadeOut => $"{this.FadeType} {this.Time}초 {endColor}",
            _ => $"invalid type:{this.FadeType}",
        };
    }

    internal void WriteTo(CutOutputJsonFormat output)
    {
        if (this.FadeType == BgFadeType.FadeIn)
        {
            output.BgFadeInStartCol = CutOutputJsonFormat.ConvertColor(this.StartColor);
            output.BgFadeInCol = CutOutputJsonFormat.ConvertColor(this.EndColor);
            output.BgFadeInTime = CutOutputJsonFormat.EliminateZero(this.Time);
        }
        else
        {
            output.BgFadeOutCol = CutOutputJsonFormat.ConvertColor(this.EndColor);
            output.BgFadeOutTime = CutOutputJsonFormat.EliminateZero(this.Time);
        }
    }

    //// --------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.FadeType):
            case nameof(this.Time):
                this.OnPropertyChanged(nameof(this.ButtonText));
                break;
        }
    }
}
