namespace CutEditor.ViewModel.Detail;

using System.Collections.Generic;
using CutEditor.Model;

/// <summary>
/// 우선은 로딩할 때마다 신규 발급. 차후에는 파일에 확정된 uid를 적고 로딩해야 한다. 
/// </summary>
internal sealed class ChoiceUidGenerator(long cutUid)
{
    private long uidSeed = 0;

    public void Initialize(IEnumerable<ChoiceOption> choices)
    {
        foreach (var choice in choices)
        {
            choice.InitializeUid(cutUid, this.GenerateNewUid());
        }
    }

    public long GenerateNewUid() => ++this.uidSeed;
}
