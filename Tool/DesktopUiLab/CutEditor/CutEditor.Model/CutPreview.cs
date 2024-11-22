namespace CutEditor.Model;

using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using Cs.Logging;
using NKM;
using Shared.Templet.TempletTypes;

public sealed class CutPreview : ObservableObject
{
    private const int UnitSlotCount = 9;

    private readonly Cut owner;
    private readonly Unit?[] units = new Unit[UnitSlotCount];
    private CameraOffset cameraOffset;
    private string? bgFileName;

    public CutPreview(Cut owner)
    {
        this.owner = owner;
    }

    public CameraOffset CameraOffset
    {
        get => this.cameraOffset;
        set => this.SetProperty(ref this.cameraOffset, value);
    }

    public string? BgFileName
    {
        get => this.bgFileName;
        set => this.SetProperty(ref this.bgFileName, value);
    }

    ////public Unit? UnitPos1
    ////{
    ////    get => this.units[0];
    ////    set => this.SetUnit(value, CutsceneUnitPos.POS1);
    ////}

    public string ValueText => this.ToString();
    private string DebugName => $"[Preview.{this.owner.Uid}]";

    public void Calculate(Cut? prevCut)
    {
        if (prevCut is null)
        {
            // 첫 번째 데이터인 경우: 컷에 설정된 값만 참조.
            this.SetUnit(this.owner.Unit, this.owner.UnitPos);
            this.SetCamera(this.owner.CameraOffset);
            this.BgFileName = this.owner.BgFileName;
            this.OnPropertyChanged(nameof(this.ValueText));
            return;
        }

        // --- 유닛 설정
        bool updated = this.CalculateUnit(prevCut);

        // --- 카메라 설정
        if (this.owner.CameraOffset != CameraOffset.NONE)
        {
            updated |= this.SetCamera(this.owner.CameraOffset);
        }
        else if (this.owner.BgFileName is not null)
        {
            updated |= this.SetCamera(CameraOffset.TWIN_6);
        }
        else
        {
            updated |= this.SetCamera(prevCut.Preview.CameraOffset);
        }

        // --- 배경 설정
        if (this.owner.BgFileName is not null)
        {
            updated |= this.SetBgFileName(this.owner.BgFileName);
        }
        else
        {
            updated |= this.SetBgFileName(prevCut.Preview.BgFileName);
        }

        if (updated)
        {
            this.OnPropertyChanged(nameof(this.ValueText));
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < this.units.Length; i++)
        {
            var unit = this.units[i];
            if (unit is null)
            {
                continue;
            }

            var position = i + CutsceneUnitPos.POS1;
            sb.AppendLine($"{position} : {unit.Name}");
        }

        return sb.ToString();
    }

    //// --------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        //switch (e.PropertyName)
        //{
        //    case nameof(this.BgCrashTime):
        //        this.OnPropertyChanged(nameof(this.HasBgFlashCrashData));
        //        break;
        //}
    }

    private bool SetUnit(Unit? unit, CutsceneUnitPos position)
    {
        if (position == CutsceneUnitPos.NONE)
        {
            position = CutsceneUnitPos.POS3;
        }

        int index = position - CutsceneUnitPos.POS1;
        if (this.units[index] == unit)
        {
            return false;
        }

        Log.Debug($"{this.DebugName} position:{position} unit:{this.units[index]?.Name} -> {unit?.Name}");
        this.units[index] = unit;
        return true;
    }

    private bool SetCamera(CameraOffset cameraOffset)
    {
        if (cameraOffset == CameraOffset.NONE)
        {
            cameraOffset = CameraOffset.TWIN_6;
        }

        if (cameraOffset == this.cameraOffset)
        {
            return false;
        }

        Log.Debug($"{this.DebugName} cameraoffset:{this.cameraOffset} -> {cameraOffset}");
        this.CameraOffset = cameraOffset;

        return true;
    }

    private bool SetBgFileName(string? bgFileName)
    {
        if (this.bgFileName == bgFileName)
        {
            return false;
        }

        Log.Debug($"{this.DebugName} bgFileName:{this.bgFileName} -> {bgFileName}");
        this.BgFileName = bgFileName;
        return true;
    }

    private bool CalculateUnit(Cut prevCut)
    {
        // 기본값 : 이전 컷의 데이터 사본.
        var buffer = prevCut.Preview.units.ToArray();

        if (this.owner.CutsceneClear == CutsceneClearType.CLEARUNIT ||
            this.owner.CutsceneClear == CutsceneClearType.CLEARALL)
        {
            if (this.owner.Unit is null)
            {
                // 모든 유닛 비우기
                Array.Clear(buffer, 0, buffer.Length);
            }
            else
            {
                // 단일 유닛 비우기
                int index = GetIndex(buffer, this.owner.Unit);
                if (index < 0)
                {
                    Log.Warn($"{this.DebugName} Invalid unit: {this.owner.Unit.Name}");
                    return false;
                }

                buffer[index] = null;
            }
        }

        if (this.owner.Unit is not null && buffer.All(e => e != this.owner.Unit))
        {
            var position = this.owner.UnitPos == CutsceneUnitPos.NONE ? CutsceneUnitPos.POS3 : this.owner.UnitPos;
            buffer[position - CutsceneUnitPos.POS1] = this.owner.Unit;
        }

        bool updated = false;
        for (int i = 0; i < buffer.Length; i++)
        {
            updated |= this.SetUnit(buffer[i], CutsceneUnitPos.POS1 + i);
        }

        return updated;

        static int GetIndex(Unit?[] units, Unit unit)
        {
            for (int i = 0; i < units.Length; i++)
            {
                if (units[i] == unit)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
