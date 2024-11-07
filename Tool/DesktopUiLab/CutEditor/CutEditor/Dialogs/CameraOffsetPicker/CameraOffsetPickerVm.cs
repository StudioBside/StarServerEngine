namespace CutEditor.Dialogs.CameraOffsetPicker;

using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Services;
using NKM;
using static CutEditor.Model.Enums;

public sealed class CameraOffsetPickerVm : ObservableObject
{
    private CameraOffsetCategory category;
    private int position = 1;
    private CameraOffset result;

    public CameraOffsetPickerVm(CameraOffset current)
    {
        this.result = current;
        (this.category, this.position) = ParseResult(current);
        this.SetCategoryCommand = new RelayCommand<string>(this.OnSetCategory);
        this.SetPositionCommand = new RelayCommand<string>(this.OnSetPosition);
    }

    public ICommand SetCategoryCommand { get; }
    public ICommand SetPositionCommand { get; }

    public CameraOffsetCategory Category
    {
        get => this.category;
        set => this.SetProperty(ref this.category, value);
    }

    public int Position
    {
        get => this.position;
        set => this.SetProperty(ref this.position, value);
    }

    public CameraOffset Result
    {
        get => this.result;
        set => this.SetProperty(ref this.result, value);
    }

    public ImageSource ResultPreview => CameraOffsetController.Instance.GetImageSource(this.result);

    //// --------------------------------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.Result):
                this.OnPropertyChanged(nameof(this.ResultPreview));
                break;

            case nameof(this.Position):
                this.Result = this.CalcResult();
                break;
        }
    }

    private static (CameraOffsetCategory Category, int Position) ParseResult(CameraOffset offset)
    {
        switch (offset)
        {
            case CameraOffset.NONE: return (CameraOffsetCategory.None, 1);
            case CameraOffset.DEFAULT: return (CameraOffsetCategory.Default, 1);
            case CameraOffset.ALL: return (CameraOffsetCategory.All, 1);

            case CameraOffset.ONE_1:
            case CameraOffset.ONE_2:
            case CameraOffset.ONE_3:
            case CameraOffset.ONE_4:
            case CameraOffset.ONE_5:
            case CameraOffset.ONE_6:
            case CameraOffset.ONE_7:
            case CameraOffset.ONE_8:
            case CameraOffset.ONE_9:
                return (CameraOffsetCategory.One, (int)offset - (int)CameraOffset.ONE_1 + 1);

            case CameraOffset.TWIN_1:
            case CameraOffset.TWIN_2:
            case CameraOffset.TWIN_3:
            case CameraOffset.TWIN_4:
            case CameraOffset.TWIN_5:
            case CameraOffset.TWIN_6:
            case CameraOffset.TWIN_7:
                return (CameraOffsetCategory.Twin, (int)offset - (int)CameraOffset.TWIN_1 + 1);

            case CameraOffset.TRIPLE_1:
            case CameraOffset.TRIPLE_2:
            case CameraOffset.TRIPLE_3:
            case CameraOffset.TRIPLE_4:
            case CameraOffset.TRIPLE_5:
                return (CameraOffsetCategory.Triple, (int)offset - (int)CameraOffset.TRIPLE_1 + 1);

            case CameraOffset.POS1_2X:
            case CameraOffset.POS2_2X:
            case CameraOffset.POS3_2X:
            case CameraOffset.POS4_2X:
            case CameraOffset.POS5_2X:
            case CameraOffset.POS6_2X:
            case CameraOffset.POS7_2X:
            case CameraOffset.POS8_2X:
            case CameraOffset.POS9_2X:
                return (CameraOffsetCategory.Pos2x, (int)offset - (int)CameraOffset.POS1_2X + 1);

            default: return (CameraOffsetCategory.None, 1);
        }
    }

    private void OnSetCategory(string? categoryStr)
    {
        if (Enum.TryParse<CameraOffsetCategory>(categoryStr, out var category) == false)
        {
            Log.Error($"invalid category literal:{categoryStr}");
            return;
        }

        this.Category = category;
        if (this.position != 1)
        {
            this.Position = 1;
        }
        else
        {
            this.Result = this.CalcResult();
        }
    }

    private void OnSetPosition(string? positionStr)
    {
        if (int.TryParse(positionStr, out var position) == false)
        {
            Log.Error($"invalid position literal:{positionStr}");
            return;
        }

        this.Position = position;
    }

    private CameraOffset CalcResult()
    {
        switch (this.category)
        {
            case CameraOffsetCategory.None:
                return CameraOffset.NONE;

            case CameraOffsetCategory.Default:
                return CameraOffset.DEFAULT;

            case CameraOffsetCategory.All:
                return CameraOffset.ALL;

            case CameraOffsetCategory.One:
            case CameraOffsetCategory.Twin:
            case CameraOffsetCategory.Triple:
                return Enum.Parse<CameraOffset>($"{this.category.ToString().ToUpper()}_{this.position}");

            case CameraOffsetCategory.Pos2x:
                return Enum.Parse<CameraOffset>($"POS{this.position}_2X");
        }

        return CameraOffset.NONE;
    }
}
