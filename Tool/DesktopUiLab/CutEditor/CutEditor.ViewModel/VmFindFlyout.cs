namespace CutEditor.ViewModel;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cs.Logging;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public sealed class VmFindFlyout : ObservableObject
{
    private readonly VmCuts owner;
    private bool isOpen;
    private string findText = string.Empty;
    private string replaceText = string.Empty;
    private int index = -1;

    public VmFindFlyout(VmCuts owner, ObservableCollection<VmCut> source)
    {
        this.owner = owner;
        this.OpenCommand = new RelayCommand(() => this.IsOpen = true);
        this.CloseCommand = new RelayCommand(() => this.IsOpen = false);
        this.FindPrevCommand = new RelayCommand(this.OnFindPrev, () => this.findText.Length > 0);
        this.FindNextCommand = new RelayCommand(this.OnFindNext, () => this.findText.Length > 0);
        this.ReplaceAllCommand = new AsyncRelayCommand(this.OnReplaceAll, () => this.findText.Length > 0);
    }

    public bool IsOpen
    {
        get => this.isOpen;
        set => this.SetProperty(ref this.isOpen, value);
    }

    public string FindText
    {
        get => this.findText;
        set => this.SetProperty(ref this.findText, value);
    }

    public string ReplaceText
    {
        get => this.replaceText;
        set => this.SetProperty(ref this.replaceText, value);
    }

    public ICommand OpenCommand { get; }
    public ICommand CloseCommand { get; }
    public IRelayCommand FindPrevCommand { get; }
    public IRelayCommand FindNextCommand { get; }
    public IRelayCommand ReplaceAllCommand { get; }

    //// --------------------------------------------------------------------

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        switch (e.PropertyName)
        {
            case nameof(this.IsOpen):
                if (this.IsOpen)
                {
                    // reset the focus to the find text box
                }
                else
                {
                    this.Reset();
                }

                break;

            case nameof(this.FindText):
                this.FindPrevCommand.NotifyCanExecuteChanged();
                this.FindNextCommand.NotifyCanExecuteChanged();
                this.ReplaceAllCommand.NotifyCanExecuteChanged();
                break;
        }
    }

    private void Reset()
    {
        //this.FindText = string.Empty;
        //this.ReplaceText = string.Empty;
        this.index = -1;
    }

    private void OnFindPrev()
    {
        var controller = this.owner.Services.GetRequiredService<ICutsListController>();
        for (int i = this.index - 1; i >= 0; i--)
        {
            if (this.owner.Cuts[i].Cut.ContainsSearchKeyword(this.findText))
            {
                this.owner.SelectedCuts.Clear();
                this.owner.SelectedCuts.Add(this.owner.Cuts[i]);
                this.index = i;
                controller.ScrollIntoView(i);
                return;
            }
        }

        Log.Info($"[이전 찾기:{this.FindText}] 더이상 찾을 수 없습니다. 현재 인덱스:{this.index}");
    }

    private void OnFindNext()
    {
        var controller = this.owner.Services.GetRequiredService<ICutsListController>();
        for (int i = this.index + 1; i < this.owner.Cuts.Count; i++)
        {
            if (this.owner.Cuts[i].Cut.ContainsSearchKeyword(this.findText))
            {
                this.owner.SelectedCuts.Clear();
                this.owner.SelectedCuts.Add(this.owner.Cuts[i]);
                this.index = i;
                controller.ScrollIntoView(i);
                return;
            }
        }

        Log.Info($"[다음 찾기:{this.FindText}] 더이상 찾을 수 없습니다. 현재 인덱스:{this.index}");
    }

    private async Task OnReplaceAll()
    {
        var boolProvider = this.owner.Services.GetRequiredService<IUserInputProvider<bool>>();
        if (await boolProvider.PromptAsync("모두 바꾸기", $"변환을 진행합니다 : [{this.findText} -> {this.replaceText}]") == false)
        {
            return;
        }

        int count = 0;
        foreach (var data in this.owner.Cuts)
        {
            if (data.Cut.TryReplaceText(this.findText, this.replaceText))
            {
                count++;
            }
        }

        var resultText = count == 0 
            ? "바꿀 내용이 없습니다."
            : $"{count}개의 항목이 변경되었습니다.";

        var popupNotifier = this.owner.Services.GetRequiredService<IPopupMessageNotifier>();
        popupNotifier.Notify("모두 바꾸기", resultText);
    }
}
