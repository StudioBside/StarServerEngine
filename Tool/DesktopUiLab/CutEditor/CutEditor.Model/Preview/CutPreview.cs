namespace CutEditor.Model.Preview;

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
    private readonly PreviewUnitSlot[] slots = new PreviewUnitSlot[UnitSlotCount];
    private CameraOffset cameraOffset;
    private string? bgFileName;

    public CutPreview(Cut owner)
    {
        this.owner = owner;

        for (int i = 0; i < this.slots.Length; i++)
        {
            this.slots[i] = new PreviewUnitSlot(CutsceneUnitPos.POS1 + i);
        }
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

    public PreviewUnitSlot UnitPos1 => this.slots[0];
    public PreviewUnitSlot UnitPos2 => this.slots[1];
    public PreviewUnitSlot UnitPos3 => this.slots[2];
    public PreviewUnitSlot UnitPos4 => this.slots[3];
    public PreviewUnitSlot UnitPos5 => this.slots[4];
    public PreviewUnitSlot UnitPos6 => this.slots[5];
    public PreviewUnitSlot UnitPos7 => this.slots[6];
    public PreviewUnitSlot UnitPos8 => this.slots[7];
    public PreviewUnitSlot UnitPos9 => this.slots[8];

    private string DebugName => $"[Preview.{this.owner.Uid}]";

    public void Calculate(Cut? prevCut)
    {
        if (prevCut is null)
        {
            // 첫 번째 데이터인 경우: 컷에 설정된 값만 참조.
            this.SetUnit(this.owner.Unit, this.owner.UnitPos);
            this.SetCamera(this.owner.CameraOffset);
            this.BgFileName = this.owner.BgFileName;
            return;
        }

        // --- 유닛 설정
        this.CalculateUnit(prevCut);

        // --- 카메라 설정
        if (this.owner.CameraOffset != CameraOffset.NONE)
        {
            this.SetCamera(this.owner.CameraOffset);
        }
        else if (this.owner.BgFileName is not null)
        {
            this.SetCamera(CameraOffset.TWIN_6);
        }
        else
        {
            this.SetCamera(prevCut.Preview.CameraOffset);
        }

        // --- 배경 설정
        if (this.owner.BgFileName is not null)
        {
            this.SetBgFileName(this.owner.BgFileName);
        }
        else
        {
            this.SetBgFileName(prevCut.Preview.BgFileName);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var slot in this.slots.Where(e => e.Unit is not null))
        {
            sb.AppendLine($"{slot.Position} : {slot.Unit!.Name}");
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

    private void SetUnit(Unit? unit, CutsceneUnitPos position)
    {
        if (position == CutsceneUnitPos.NONE)
        {
            position = CutsceneUnitPos.POS3;
        }

        int index = position - CutsceneUnitPos.POS1;
        if (this.slots[index].Unit == unit)
        {
            return;
        }

        Log.Debug($"{this.DebugName} position:{position} unit:{this.slots[index].Unit?.Name} -> {unit?.Name}");
        this.slots[index].Unit = unit;
        this.OnPropertyChanged($"UnitPos{index + 1}");
    }

    private void SetCamera(CameraOffset cameraOffset)
    {
        if (cameraOffset == CameraOffset.NONE)
        {
            cameraOffset = CameraOffset.TWIN_6;
        }

        if (cameraOffset == this.cameraOffset)
        {
            return;
        }

        Log.Debug($"{this.DebugName} cameraoffset:{this.cameraOffset} -> {cameraOffset}");
        this.CameraOffset = cameraOffset;
    }

    private void SetBgFileName(string? bgFileName)
    {
        if (this.bgFileName == bgFileName)
        {
            return;
        }

        Log.Debug($"{this.DebugName} bgFileName:{this.bgFileName} -> {bgFileName}");
        this.BgFileName = bgFileName;
    }

    private void CalculateUnit(Cut prevCut)
    {
        // 기본값 : 이전 컷의 데이터 사본.
        var buffer = prevCut.Preview.slots.Select(e => e.Unit).ToArray();

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
                    return;
                }

                buffer[index] = null;
            }
        }
        else if (this.owner.Unit is not null)
        {
            var prevIndex = GetIndex(buffer, this.owner.Unit);
            if (prevIndex < 0)
            {
                // 기존에 화면에 나와있지 않은 유닛을 지정 : 새로 등장
                var position = this.owner.UnitPos == CutsceneUnitPos.NONE ? CutsceneUnitPos.POS3 : this.owner.UnitPos;
                buffer[position - CutsceneUnitPos.POS1] = this.owner.Unit;
            }
            else if (this.owner.UnitPos != CutsceneUnitPos.NONE)
            {
                // 기존에 화면에 나와있는 유닛과 위치값을 지정 : 위치 변경
                buffer[prevIndex] = null;
                buffer[this.owner.UnitPos - CutsceneUnitPos.POS1] = this.owner.Unit;
            }
        }

        for (int i = 0; i < buffer.Length; i++)
        {
            this.SetUnit(buffer[i], CutsceneUnitPos.POS1 + i);
        }

        static int GetIndex(Unit?[] slots, Unit unit)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == unit)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
