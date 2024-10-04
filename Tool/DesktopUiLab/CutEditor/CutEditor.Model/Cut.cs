namespace CutEditor.Model;

using System;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Core.Util;
using Cs.Logging;
using Newtonsoft.Json.Linq;
using Shared.Templet.Base;
using Shared.Templet.TempletTypes;

public sealed class Cut : ObservableObject
{
    private readonly L10nText unitTalk = new();
    private string unitName = string.Empty;
    private float talkTime;
    private string unitStrId = string.Empty;
    private Unit? unit;

    public Cut(JToken token, long uid)
    {
        this.Uid = uid;
        this.unitTalk.Load(token, "UnitTalk");
        this.unitName = token.GetString("UnitNameString", string.Empty);
        this.talkTime = token.GetFloat("TalkTime", 0f);
        this.unitStrId = token.GetString("UnitStrId", string.Empty);

        if (string.IsNullOrEmpty(this.unitStrId) == false)
        {
            this.unit = TempletContainer<Unit>.Find(this.unitStrId);
            if (this.unit is null)
            {
                Log.Error($"유닛 템플릿을 찾을 수 없습니다. UnitStrId:{this.unitStrId}");
            }
        }
    }

    public long Uid { get; }
    public L10nText UnitTalk => this.unitTalk;
    public string UnitName
    {
        get => this.unitName;
        set => this.SetProperty(ref this.unitName, value);
    }

    public float TalkTime
    {
        get => this.talkTime;
        set => this.SetProperty(ref this.talkTime, value);
    }

    public Unit? Unit
    {
        get => this.unit;
        set => this.SetProperty(ref this.unit, value);
    }

    //// --------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.Unit):
                this.ResetUnitStrId();
                break;
        }
    }

    private void ResetUnitStrId()
    {
        if (this.unit is null)
        {
            this.unitStrId = string.Empty;
            return;
        }

        this.unitStrId = this.unit.StrId;
    }
}
