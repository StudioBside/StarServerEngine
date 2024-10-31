namespace CutEditor.ViewModel.Detail;

using System.Collections.Generic;
using CutEditor.Model;

internal sealed class ChoiceUidGenerator(long cutUid)
{
    private long uidSeed = 0;

    public void Initialize(IEnumerable<ChoiceOption> choices)
    {
        if (choices.Any() == false)
        {
            return;
        }

        bool newFormat = choices.First().ChoiceUid > 0;
        if (newFormat)
        {
            // 이미 로딩한 데이터에 uid가 있는 경우 (=신규 포맷) : seed를 조정.
            this.uidSeed = choices.Max(x => x.ChoiceUid);
        }

        foreach (var choice in choices)
        {
            var choiceUid = newFormat ? choice.ChoiceUid : this.Generate();
            choice.InitializeUid(cutUid, choiceUid);
        }
    }

    public long Generate() => ++this.uidSeed;
}
