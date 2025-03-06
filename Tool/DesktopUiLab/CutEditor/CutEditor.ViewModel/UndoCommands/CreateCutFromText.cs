namespace CutEditor.ViewModel.UndoCommands;

using System.Text;
using CutEditor.Model;
using CutEditor.Model.Interfaces;
using Du.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Shared.Templet.TempletTypes;

internal sealed class CreateCutFromText : IDormammu
{
    private readonly VmCuts vmCuts;
    private readonly IReadOnlyList<VmCut> targets;
    private readonly int positionIndex;

    private CreateCutFromText(VmCuts vmCuts, IReadOnlyList<VmCut> targets, int positionIndex)
    {
        this.vmCuts = vmCuts;
        this.targets = targets;
        this.positionIndex = positionIndex;
    }

    public static async Task<CreateCutFromText?> Create(VmCuts vmCuts, string text)
    {
        text = text.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var sb = new StringBuilder();
        var tokens = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        sb.AppendLine($"다음의 텍스트를 이용해 {tokens.Length}개의 cut 데이터를 생성합니다.");
        sb.AppendLine();
        int previewCharacterCount = 60;
        if (text.Length > previewCharacterCount)
        {
            sb.AppendLine($"{text[..previewCharacterCount]} ... (and {text.Length - previewCharacterCount} more)");
        }
        else
        {
            sb.AppendLine(text);
        }

        var boolProvider = vmCuts.Services.GetRequiredService<IUserInputProvider<bool>>();
        if (await boolProvider.PromptAsync("새로운 Cut을 만듭니다", sb.ToString()) == false)
        {
            return null;
        }

        int positionIndex = vmCuts.Cuts.Count - 1;
        if (vmCuts.SelectedCuts.Count > 0)
        {
            positionIndex = vmCuts.Cuts.IndexOf(vmCuts.SelectedCuts[^1]);
        }

        var targets = new List<VmCut>();
        foreach (var token in tokens)
        {
            // 유닛이름 : 텍스트 형식인 경우, 이름을 파싱해 유효한 값인지 확인
            Unit? unit = null;
            string talkText = token;
            var idx = token.IndexOf(':');
            if (idx > 0)
            {
                var unitName = token[..idx].Trim();
                unit = Unit.Values.FirstOrDefault(e => e.Name == unitName);
                if (unit is not null)
                {
                    talkText = token[(idx + 1)..].Trim();
                }
            }

            var cut = vmCuts.CreateNewCut();
            cut.Unit = unit;

            // < ~ > 로 둘러싸인 경우 선택지 포맷으로 인식
            if (unit is null && talkText.StartsWith('<') && talkText.EndsWith('>'))
            {
                var newChoice = new ChoiceOption(cut.Uid, choiceUid: 1, talkText[1..^1]);
                cut.Choices.Add(newChoice);
            }
            else
            {
                // 아닐 땐 일반 unitTalk.
                cut.UnitTalk.Korean = talkText;
                cut.TalkTime = Cut.TalkTimeDefault;
            }

            targets.Add(new VmCut(cut, vmCuts));
        }

        return new CreateCutFromText(vmCuts, targets, positionIndex);
    }

    public void Redo()
    {
        int index = this.positionIndex + 1;

        foreach (var cut in this.targets)
        {
            this.vmCuts.Cuts.Insert(index, cut);
            index++;
        }

        this.vmCuts.SelectedCuts.Clear();
        foreach (var cut in this.targets)
        {
            this.vmCuts.SelectedCuts.Add(cut);
        }

        var controller = this.vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(index);

        this.vmCuts.CutPaster.ClearReserved();
    }

    public void Undo()
    {
        // 삭제된 후 선택될 항목을 결정.
        int selectedIndex = this.vmCuts.Cuts.IndexOf(this.targets[0]);
        if (selectedIndex > 0)
        {
            selectedIndex--;
        }

        foreach (var cut in this.targets)
        {
            this.vmCuts.Cuts.Remove(cut);
        }

        var selected = this.vmCuts.Cuts[selectedIndex];
        this.vmCuts.SelectedCuts.Clear();
        this.vmCuts.SelectedCuts.Add(this.vmCuts.Cuts[selectedIndex]);

        var controller = this.vmCuts.Services.GetRequiredService<ICutsListController>();
        controller.FocusElement(selectedIndex);
    }
}
